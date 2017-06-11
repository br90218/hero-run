// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//  Copyright(c) 2016, Michal Skalsky
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its contributors
//     may be used to endorse or promote products derived from this software without
//     specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT
//  SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
//  OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
//  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//Modified by Carlos Macarrón & David Miranda

Shader "Hidden/Upsample"
{
	Properties
	{
		_UpsampleDepthThreshold("Upsample Depth Threshold", Range(0, 2)) = .05
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE

	 
		
	   // #define UPSAMPLE_DEPTH_THRESHOLD 0.05f
		uniform half _UpsampleDepthThreshold = .05;
       
     

		#define PI 3.1415927f

#include "UnityCG.cginc"	

        float4 _TexelSize;
		sampler2D _HiResDepthBuffer;
		sampler2D _LowResDepthBuffer;
        sampler2D _LowResColor;

		struct appdata
		{

			float4 vertex : POSITION;

			float2 uv : TEXCOORD0;
		};

		struct v2fDownsample
		{
			float2 uv : TEXCOORD0;
			float2 uv00 : TEXCOORD1;
			float2 uv01 : TEXCOORD2;
			float2 uv10 : TEXCOORD3;
			float2 uv11 : TEXCOORD4;
			float4 vertex : SV_POSITION;
		};

		struct v2fUpsample
		{
			float2 uv : TEXCOORD0;
			float2 uv00 : TEXCOORD1;
			float2 uv01 : TEXCOORD2;
			float2 uv10 : TEXCOORD3;
			float2 uv11 : TEXCOORD4;
			float4 vertex : SV_POSITION;
		};

		//-----------------------------------------------------------------------------------------
		// vertDownsampleDepth
		//-----------------------------------------------------------------------------------------
		v2fDownsample vertDownsampleDepth(appdata v, float2 texelSize)
		{
			v2fDownsample o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;

            o.uv00 = v.uv - 0.5 * texelSize.xy;
            o.uv10 = o.uv00 + float2(texelSize.x, 0);
            o.uv01 = o.uv00 + float2(0, texelSize.y);
            o.uv11 = o.uv00 + texelSize.xy;
			return o;
		}

		//-----------------------------------------------------------------------------------------
		// vertUpsample
		//-----------------------------------------------------------------------------------------
        v2fUpsample vertUpsample(appdata v, float2 texelSize)
        {
            v2fUpsample o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;

            o.uv00 = v.uv - 0.5 * texelSize.xy;
            o.uv10 = o.uv00 + float2(texelSize.x, 0);
            o.uv01 = o.uv00 + float2(0, texelSize.y);
            o.uv11 = o.uv00 + texelSize.xy;
            return o;
        }

		#undef SAMPLE_DEPTH_TEXTURE
		#define SAMPLE_DEPTH_TEXTURE(sampler, uv) (  tex2D(sampler, uv)  )
		
		//-----------------------------------------------------------------------------------------
		// BilateralUpsample
		//-----------------------------------------------------------------------------------------
		float4 BilateralUpsample(v2fUpsample input, sampler2D hiDepth, sampler2D loDepth, sampler2D loColor)
		{
			float4 highResDepth = (SAMPLE_DEPTH_TEXTURE(hiDepth, input.uv)).xxxx;
			float4 lowResDepth=0;

			lowResDepth[0] = (SAMPLE_DEPTH_TEXTURE(loDepth, input.uv00));
			lowResDepth[1] = (SAMPLE_DEPTH_TEXTURE(loDepth, input.uv10));
			lowResDepth[2] = (SAMPLE_DEPTH_TEXTURE(loDepth, input.uv01));
			lowResDepth[3] = (SAMPLE_DEPTH_TEXTURE(loDepth, input.uv11));

			float4 depthDiff = abs(lowResDepth - highResDepth);
			float accumDiff = dot(depthDiff, float4(1, 1, 1, 1));
			UNITY_BRANCH
			if (accumDiff < _UpsampleDepthThreshold) // small error, not an edge -> use bilinear filter
			{
				// should be bilinear sampler, dont know how to use two different samplers for one texture in Unity
				//return float4(1, 0, 0, 1);
				return tex2D(loColor, input.uv);
			}

			#if VISUALIZE_EDGE
				return float4(1, .2, 0, 1);
			#endif

			// find nearest sample
			float minDepthDiff = depthDiff[0];
			float2 nearestUv = input.uv00;

			if (depthDiff[1] < minDepthDiff)
			{
				nearestUv = input.uv10;
				minDepthDiff = depthDiff[1];
			}

			if (depthDiff[2] < minDepthDiff)
			{
				nearestUv = input.uv01;
				minDepthDiff = depthDiff[2];
			}

			if (depthDiff[3] < minDepthDiff)
			{
				nearestUv = input.uv11;
				minDepthDiff = depthDiff[3];
			}

			return tex2D(loColor, nearestUv);
		}

		//-----------------------------------------------------------------------------------------
		// DownsampleDepth
		//-----------------------------------------------------------------------------------------
		float4 DownsampleDepth(v2fDownsample input, sampler2D depthTexture)
		{
			float final;
			// Normal
			final = tex2D(depthTexture, input.uv).x;

#if DOWNSAMPLE_DEPTH_MODE_MIN // min  depth
			float depth =  tex2D(depthTexture, input.uv00) ;
			depth = min(depth,  tex2D(depthTexture, input.uv01) );
			depth = min(depth,  tex2D(depthTexture, input.uv10) );
			depth = min(depth,  tex2D(depthTexture, input.uv11) );
			final = (depth);
#elif DOWNSAMPLE_DEPTH_MODE_MAX // max  depth
            float depth =  tex2D(depthTexture, input.uv00) ;
            depth = max(depth,  tex2D(depthTexture, input.uv01) );
			depth = max(depth,  tex2D(depthTexture, input.uv10) );
			depth = max(depth,  tex2D(depthTexture, input.uv11) );
			final = (depth);
#else	// DOWNSAMPLE_DEPTH_MODE_CHESSBOARD
			float4 depth;
			depth.x =  tex2D(depthTexture, input.uv00) ;
			depth.y =  tex2D(depthTexture, input.uv01) ;
			depth.z =  tex2D(depthTexture, input.uv10) ;
			depth.w =  tex2D(depthTexture, input.uv11) ;

			float minDepth = min(min(depth.x, depth.y), min(depth.z, depth.w));
			float maxDepth = max(max(depth.x, depth.y), max(depth.z, depth.w));

			// chessboard pattern
			int2 position = input.vertex.xy % 2;
			int index = position.x + position.y;
			final = (index == 1 ? minDepth : maxDepth);
#endif
			return final;
		}

		ENDCG

		// pass 0 - downsample depth
		Pass
		{
			CGPROGRAM
			#pragma vertex vertDepth
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile DOWNSAMPLE_DEPTH_MODE_MIN DOWNSAMPLE_DEPTH_MODE_MAX DOWNSAMPLE_DEPTH_MODE_CHESSBOARD

			v2fDownsample vertDepth(appdata v)
			{
				return vertDownsampleDepth(v, _TexelSize);
			}

			float4 frag(v2fDownsample input) : SV_Target
			{
				return DownsampleDepth(input, _HiResDepthBuffer);
			}

			ENDCG
		}

		// pass 1 - bilateral upsample
		Pass
		{
			Blend One Zero

			CGPROGRAM
			#pragma vertex vertUpsampleToFull
			#pragma fragment frag		
			#pragma target 3.0
			#pragma shader_feature VISUALIZE_EDGE

			v2fUpsample vertUpsampleToFull(appdata v)
			{
				return vertUpsample(v, _TexelSize);
			}
			float4 frag(v2fUpsample input) : SV_Target
			{
				return BilateralUpsample(input, _HiResDepthBuffer, _LowResDepthBuffer, _LowResColor);
			}

			ENDCG
		}
	}
}
