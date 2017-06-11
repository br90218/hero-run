Shader "Hidden/FogVolume"
{
	Properties
	{
		_SrcBlend("__src", Float) = 1.0
	}
		CGINCLUDE
		#include "UnityCG.cginc" 
		#include "CommonInputs.cginc"
		half NoiseAtten = 1;
		int _SrcBlend;
		int _ztest;
		#include "FogVolumeCommon.cginc"
		#define DEBUG_ITERATIONS 1
		#define DEBUG_INSCATTERING 2
		#define DEBUG_VOLUMETRIC_SHADOWS 3
		#define DEBUG_VOLUME_FOG_INSCATTER_CLAMP 4
		#define DEBUG_VOLUME_FOG_PHASE 5
		#include "FogVolumeFragment.cginc"
	

		ENDCG

	//normal pass
	SubShader
	{ 
			Stencil
		{
			Ref 2
			Comp Always
			Pass Replace
		}
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True"}
		LOD 600
		//Blend SrcAlpha OneMinusSrcAlpha
			//Blend One OneMinusSrcAlpha 
			Blend[_SrcBlend] OneMinusSrcAlpha 

		Fog{ Mode Off }
		Cull Front
		Lighting Off
		ZWrite Off
		ZTest [_ztest]

	Pass 
	{
		

	CGPROGRAM
	
	#pragma multi_compile _ _FOG_LOWRES_RENDERER 			
	#pragma shader_feature _INSCATTERING  
	#pragma shader_feature VOLUME_FOG
	#pragma shader_feature _VOLUME_FOG_INSCATTERING  
	#pragma shader_feature _FOG_GRADIENT  
	#pragma shader_feature _FOG_VOLUME_NOISE    
	#pragma shader_feature _COLLISION            
	//#pragma multi_compile _ DEBUG
	//#pragma shader_feature  SAMPLING_METHOD_ViewAligned
	#pragma shader_feature HEIGHT_GRAD
	#pragma shader_feature _TONEMAP	
	#pragma shader_feature JITTER
	#pragma shader_feature ColorAdjust	
	#pragma shader_feature	ABSORPTION			
	#pragma multi_compile _ Twirl_X Twirl_Y Twirl_Z
	#pragma shader_feature _SHADE
	#pragma shader_feature DF
	#pragma shader_feature DIRECTIONAL_LIGHTING 
	#pragma multi_compile _ ATTEN_METHOD_1 ATTEN_METHOD_2 ATTEN_METHOD_3
	//#pragma shader_feature SPHERICAL_FADE
	//#pragma shader_feature _LAMBERT_SHADING
	#pragma multi_compile _ VOLUME_SHADOWS
	#pragma shader_feature LIGHT_ATTACHED
	#pragma shader_feature HALO
	#pragma shader_feature RENDER_SCENE_VIEW

	//#pragma exclude_renderers d3d9
	#pragma glsl//they say it's not needed anymoar https://docs.unity3d.com/Manual/SL-ShaderPrograms.html
	#pragma vertex vert
	#pragma fragment frag	

	#pragma target 3.0
	
	ENDCG
	}

	}
	
//opacity pass . Only for shadow rt
		SubShader
		{
			Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" }
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha

			Fog{ Mode Off }
			Cull Front
			Lighting Off
			ZWrite Off
			ZTest Always

			Pass 
			{
			Fog{ Mode Off }			

			CGPROGRAM

			#pragma shader_feature _FOG_GRADIENT  
			#pragma shader_feature _FOG_VOLUME_NOISE    
			#pragma shader_feature _COLLISION            
			#pragma shader_feature  SAMPLING_METHOD_ViewAligned
			#pragma shader_feature HEIGHT_GRAD
			#pragma multi_compile Twirl_X Twirl_Y Twirl_Z
			#pragma shader_feature DF
	

			//#pragma exclude_renderers d3d9
			#pragma glsl
			#pragma vertex vert
			#pragma fragment frag	

			#pragma target 3.0
			ENDCG
			}

		}
}
