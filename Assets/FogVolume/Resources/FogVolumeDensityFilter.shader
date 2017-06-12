// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FogVolumeDensityFilter" {

	Properties{
		_MainTex("", any) = "" {}
	}
		CGINCLUDE
#include "UnityCG.cginc"
#pragma target 3.0


		struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		
	};

	sampler2D _MainTex, RT_FogVolumeConvolution;
	float4 _MainTex_TexelSize;
	float4 _BlurOffsets;
	float _Dither, _Density, _Falloff, FOV_compensation;
	int Iter;
	half4 _MainTex_ST;

	float nrand(float2 ScreenUVs)
	{
		return frac(sin(ScreenUVs.x * 12.9898 + ScreenUVs.y * 78.233) * 43758.5453);
	}
	

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord - _BlurOffsets.xy * _MainTex_TexelSize.xy, _MainTex_ST);

		return o;
	}
	
	float4 frag(v2f i) : SV_Target
	{
		float4 FogVolume = tex2D(RT_FogVolumeConvolution,  i.uv);
		float FogVolumeAlpha = FogVolume.a;
		half Dither = (nrand(i.uv + frac(_Time.x)));
		FogVolumeAlpha *= lerp(1, Dither, _Dither);
		half Density = pow(FogVolumeAlpha,_Falloff)*FOV_compensation;
		
		//return Dither;
			//Density += _Dither * Dither;
		//return Density;
		float2 offset1 = _MainTex_TexelSize * _BlurOffsets.xy;
		float2 offset2 = -_MainTex_TexelSize * _BlurOffsets.xy;
		float2 offset3 = +_MainTex_TexelSize * _BlurOffsets.xy * float2(1, -1);
		float2 offset4 = -_MainTex_TexelSize * _BlurOffsets.xy * float2(1, -1);

		offset1 *= Density;
		offset2 *= Density;
		offset3 *= Density;
		offset4 *= Density;

		float4 BlurredScene = tex2D(_MainTex, i.uv + offset1);		
		BlurredScene += tex2D(_MainTex, i.uv + offset2);
		BlurredScene += tex2D(_MainTex, i.uv + offset3);
		BlurredScene += tex2D(_MainTex, i.uv + offset4);
				
		BlurredScene = BlurredScene*.25;
		
		return BlurredScene;
		
		
	}
		ENDCG
		SubShader {

		Pass{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	Fallback off
}
