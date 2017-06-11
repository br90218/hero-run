// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef FOG_VOLUME_COMMON_INPUTS_INCLUDED
#define FOG_VOLUME_COMMON_INPUTS_INCLUDED

uniform sampler2D			
							_Gradient, 
							_ValueNoise;
#ifdef _FOG_VOLUME_NOISE

uniform sampler3D			_NoiseVolume;

#endif


#ifdef DEBUG
sampler2D _PerformanceLUT;
#endif

#ifdef _FOG_LOWRES_RENDERER
sampler2D RT_Depth;
#endif

#ifdef VOLUME_SHADOWS
sampler2D	LightshaftTex;
#endif

#ifdef HALO
	sampler2D	_LightHaloTexture;
	fixed	_HaloOpticalDispersion,
		_HaloIntensity,
		_HaloWidth,
		_HaloRadius,
		_HaloAbsorption;
#endif
	sampler2D _CameraDepthTexture;
//uniform float4				_LightMapTex_ST;

	int STEP_COUNT = 200;
#ifdef DF
	uniform float4x4			_PrimitivesTransform[20];
	uniform half	_PrimitiveEdgeSoftener;
	uniform float4 _PrimitivePosition[20],
		_PrimitiveScale[20];
	int _PrimitiveCount = 0;
#endif
#if SHADER_API_GLCORE || SHADER_API_D3D11
	
#if ATTEN_METHOD_1 || ATTEN_METHOD_2 || ATTEN_METHOD_3 
uniform int _PointLightsSize[256];
uniform float4 _PointLightPositions[256],
_PointLightColors[256];
half  PointLightingDistance,
PointLightingDistance2Camera,
_PointLightsRange[256],
_PointLightsIntensity[256];

#endif
#endif
#ifdef _SHADE
int _SelfShadowSteps;
#endif
//uniform int							STEP_COUNT = 50,
							
			int				_LightsCount = 0, 
							
							
							Octaves,
							_DebugMode,
							SamplingMethod,
							DirectLightingShadowSteps;

uniform float4				_Color,
							_FogColor,
							_InscatteringColor,
							_BoxMin,
							_BoxMax,
							LightExtinctionColor = 0,
							Stretch,
							_LightColor,
							_AmbientColor,
							VolumeSize,
							VolumeFogInscatteringColor,
							_VolumePosition;

							


uniform float3				L = float3(0, 0, 1), 
							_LightLocalDirection,
							_CameraForward,
							_SliceNormal,
							Speed = 1;

uniform half				DetailTiling,	
							_PrimitiveCutout,
							HeightAbsorption,
							_LightExposure,
							VolumeFogInscatteringIntensity,
							VolumeFogInscatteringAnisotropy,
							VolumeFogInscatteringStartDistance,
							VolumeFogInscatteringTransitionWideness,
							_PushAlpha,
							_DetailMaskingThreshold,
							_DetailSamplingBaseOpacityLimit,
							_OptimizationFactor,
							_BaseRelativeSpeed,
							_DetailRelativeSpeed,
							
							_NoiseDetailRange,
							_Curl,
							Absorption, 
							BaseTiling, 
							_Cutoff,
							Coverage,
							NoiseDensity,
							LambertianBias,
							DirectLightingShadowDensity,
							DirectLightingAmount,
							NormalDistance,
							_Vortex = 1, 
							_RotationSpeed, 
							_Rotation, 
							DirectLightingDistance,
							
							_FOV,
							_DirectionalLightingDistance,
							GradMin, 
							GradMax,
							Constrain, 
							SphericalFadeDistance, 
							DetailDistance,
							_SceneIntersectionSoftness, 
							_InscatteringIntensity = 1, 
							InscatteringShape, 
							_Visibility, 
							InscatteringStartDistance = 100, 
							IntersectionThreshold,
							InscatteringTransitionWideness = 500, 
							_3DNoiseScale, 
							_RayStep, 
							gain = 1, 
							threshold = 0,
							Shade, 
							_SceneIntersectionThreshold, 
							ShadowBrightness, 
							_jitter, 
							FadeDistance, 
							Offset = 0, 
							Gamma = 1, 
							
							Exposure;


struct v2f
{
	float4 pos         : SV_POSITION;
	float3 Wpos        : TEXCOORD0;
	float4 ScreenUVs   : TEXCOORD1;
	float3 LocalPos    : TEXCOORD2;
	float3 ViewPos     : TEXCOORD3;
	float3 LocalEyePos : TEXCOORD4;
	float3 LightLocalDir : TEXCOORD5;
	float3 WorldEyeDir  : TEXCOORD6;
	float2 uv0 : TEXCOORD7;
	float3 SliceNormal : TEXCOORD8;
};


v2f vert(appdata_full i)
{
	v2f o;
	o.pos = UnityObjectToClipPos(i.vertex);
	o.Wpos = mul((float4x4)unity_ObjectToWorld, float4(i.vertex.xyz, 1)).xyz;
	o.ScreenUVs = ComputeScreenPos(o.pos);

#ifdef _FOG_LOWRES_RENDERER
#ifndef UNITY_SINGLE_PASS_STEREO
	if (unity_CameraProjection[0][2] < 0)
	{
		o.ScreenUVs.x = (o.ScreenUVs.x * 0.5f);
	}
	else if (unity_CameraProjection[0][2] > 0)
	{
		o.ScreenUVs.x = (o.ScreenUVs.x * 0.5f) + (o.ScreenUVs.w * 0.5f);
	}
#endif
#endif

	//o.ViewPos = mul((float4x4)UNITY_MATRIX_MV, float4(i.vertex.xyz, 1)).xyz;
	//5.6
	
	o.ViewPos = UnityObjectToViewPos( float4(i.vertex.xyz, 1)).xyz;
	o.LocalPos = i.vertex.xyz;
	o.LocalEyePos = mul((float4x4)unity_WorldToObject, (float4(_WorldSpaceCameraPos, 1))).xyz;
	o.LightLocalDir = mul((float4x4)unity_WorldToObject, (float4(L.xyz, 1))).xyz;
	o.WorldEyeDir = (o.Wpos.xyz - _WorldSpaceCameraPos.xyz);
	o.uv0 = i.texcoord;
	//WIN http://answers.unity3d.com/questions/192553/camera-forward-vector-in-shader.html
	o.SliceNormal = UNITY_MATRIX_IT_MV[2].xyz;// mul((float4x4)unity_WorldToObject, _SliceNormal).xyz;
	return o;

}
#endif