// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Fog Volume/Depth" 
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
	}

	CGINCLUDE
#pragma target 3.0
	#include "UnityCG.cginc"
		sampler2D _CameraDepthTexture;

	struct v2f {
		float4 vertex : SV_POSITION;
		half4 projPos  : TEXCOORD0;
		half depth:TEXCOORD1;
		//UNITY_VERTEX_OUTPUT_STEREO
	};

	float _AspectRatio;
	v2f vert(appdata_full v)
	{
		
		v2f o;
		
		o.vertex = UnityObjectToClipPos(v.vertex);
		//Is Unity doing some kind of lens correction to the GBuffer?
#if defined(UNITY_SINGLE_PASS_STEREO) || defined(STEREO_INSTANCING_ON)
		o.vertex.w -=.016;
		o.vertex.w *= .9999;
#endif
		o.projPos = ComputeScreenPos(v.vertex);

		o.depth = COMPUTE_DEPTH_01;

		return o;
	}
	half4 frag(v2f i) : COLOR{
		//float z = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos ));
		//return float4(1, 0, 0, 1);
		//return (Depth);
		//float z = Linear01Depth(i.projPos.z / i.projPos.w);
#if  UNITY_REVERSED_Z!=1
		i.depth = 1.0f - i.depth;

#endif
	float d = Linear01Depth(i.depth); 
	
		return (d);
	//return EncodeFloatRG(d).xyxy;
//	return d;
		//
		
	}
	ENDCG
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		 Pass {
			
			Fog{ Mode Off }
			  CGPROGRAM

			  #pragma vertex vert
			  #pragma fragment frag
#pragma multi_compile _ STEREO_INSTANCING_ON 
#pragma multi_compile _ UNITY_SINGLE_PASS_STEREO
			  ENDCG
		  }
	}
	Fallback off
}
