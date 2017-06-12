#ifndef FOG_VOLUME_COMMON_INCLUDED
#define FOG_VOLUME_COMMON_INCLUDED

float sdSphere(float3 p, float s)
{
	return (length(p) - s);
}

float sdBox(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	d = (min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0)));
	
		return d;			
}

float udBox(float3 p, float3 b)
{
	return length(max(abs(p) - b, 0.0));
}
float nrand(float2 ScreenUVs)
{
	return frac(sin(ScreenUVs.x * 12.9898 + ScreenUVs.y * 78.233) * 43758.5453);
}

float remap_tri(float v)
{
	float orig = v * 2.0 - 1.0;
	v = max(-1.0, orig / sqrt(abs(orig)));
	
	return v - sign(orig) + 0.5;
}

bool IntersectBox(float3 startpoint, float3 direction, float3 boxmin, float3 boxmax, out float tnear,	out float tfar)
{
	// compute intersection of ray with all six bbox planes
	float3 invR = 1.0 / direction;
	float3 tbot = invR * (boxmin.xyz - startpoint);
	float3 ttop = invR * (boxmax.xyz - startpoint);
	// re-order intersections to find smallest and largest on each axis
	float3 tmin = min(ttop, tbot);
	float3 tmax = max(ttop, tbot);
	// find the largest tmin and the smallest tmax
	float2 t0 = max(tmin.xx, tmin.yz);
	tnear = max(t0.x, t0.y);
	t0 = min(tmax.xx, tmax.yz);
	tfar = min(t0.x, t0.y);
	// check for hit
	bool hit;
	if ((tnear > tfar))
		hit = false;
	else
		hit = true;
	return hit;
}
half3 ToneMap(half3 x, half exposure)
{
	//Photographic
	return 1 - exp2(-x * exposure);
}
#define PI 3.1416
//http://zurich.disneyresearch.com/~wjarosz/publications/dissertation/chapter4.pdf
float Henyey(float3 E, float3 L, float mieDirectionalG)
{
	float theta = saturate(dot(E, L));
	return (1.0 / (4.0 * PI)) * ((1.0 - mieDirectionalG * mieDirectionalG) / pow(1.0 - 2.0 * mieDirectionalG * theta + mieDirectionalG * mieDirectionalG, 1.5));
}

half3 ContrastF(half3 pixelColor, fixed contrast)
{
	return saturate(((pixelColor.rgb - 0.5f) * max(contrast, 0)) + 0.5f);
}

float3 rotate(float3 p, float rot)
{
	float3 r = 0;
#ifdef Twirl_X
	float3x3 rx = float3x3(1.0, 0.0, 0.0, 0.0, cos(rot), sin(rot), 0.0, -sin(rot), cos(rot));
	r= mul(p, rx);
#endif 

#ifdef Twirl_Y
	float3x3 ry = float3x3(cos(rot), 0.0, -sin(rot), 0.0, 1.0, 0.0, sin(rot), 0.0, cos(rot));
	r = mul(p, ry);
#endif

#ifdef Twirl_Z
	float3x3 rz = float3x3(cos(rot), -sin(rot), 0.0, sin(rot), cos(rot), 0.0, 0.0, 0.0, 1.0);
	r = mul(p, rz);
#endif
	return r;
}

half HeightGradient(float H, half H0, half Hmax)
{
	return saturate((H - H0) / (Hmax - H0));
}

half Threshold(float a, float Gain, float Contrast)
{
	float input = a * Gain;
	//float thresh = input - Contrast;
	float thresh = ContrastF(input, Contrast);
	return saturate(lerp(0.0f, input, thresh));
}
half PrimitiveAccum = 1;
#ifdef _FOG_VOLUME_NOISE

float NoiseSamplerLoop(float3 p)
{
	float n = 0, iter = 1;

	for (int i = 0; i < Octaves; i++)
	{
		p += Speed.rgb *_BaseRelativeSpeed * (i*.15+1);
		iter *= 1.1;
		n += (tex3Dlod(_NoiseVolume, float4(p*iter, 0)))*5;
	}

	return n / Octaves;
}



float noise(in float3 p, half DistanceFade)
{
	float Volume = 0;
	float lowFreqScale = BaseTiling;


	half NoiseBaseLayers = 0;

	if (Coverage > 0.01)
		NoiseBaseLayers = NoiseSamplerLoop(p * lowFreqScale);
	/*if (Octaves > 0)
	NoiseBaseLayers += NoiseSamplerLoop(p *lowFreqScale * 2);
	if (Octaves > 1)*/
#ifdef DF
	NoiseBaseLayers = Threshold(NoiseBaseLayers, Coverage * NoiseAtten*PrimitiveAccum, threshold);
#else
	NoiseBaseLayers = Threshold(NoiseBaseLayers, Coverage * NoiseAtten, threshold);
#endif
	half NoiseDetail = 0;
	half BaseMask = saturate((1 - NoiseBaseLayers * _DetailMaskingThreshold));
	//NoiseBaseLayers = abs(NoiseBaseLayers);
	//half DetailSamplingBaseOpacityLimit = _DetailMaskingThreshold * .06;//small value that will be used to sample detail ONLY when the base layer is opaque enough
	if (DistanceFade > 0 && NoiseBaseLayers>0 && NoiseBaseLayers<_DetailSamplingBaseOpacityLimit)//no me samplees donde ya es opaco del tó
	{
		NoiseDetail += BaseMask*DistanceFade*(tex3Dlod(_NoiseVolume, float4(p*DetailTiling + Speed * _DetailRelativeSpeed, 0)).r);
		
		if (Octaves > 1 /*&& NoiseDetail<.1*/ )
			NoiseDetail += DistanceFade*(tex3Dlod(_NoiseVolume, 
				//size and offset
				float4(p * .5 * DetailTiling + .5
					//distortion
				+ NoiseDetail * _Curl * BaseMask * 2.0-1.0
					//animation
				+ Speed * _DetailRelativeSpeed, 0)).r) * 1.5 * BaseMask;

		//NoiseBaseLayers = max(0, NoiseBaseLayers - .21);
		//fade to avoid the hard edges
		//NoiseDetail *= NoiseBaseLayers*5;
		NoiseDetail= Threshold(NoiseDetail, 1, 0);
	}

//base layer (coverage)
	Volume += NoiseBaseLayers;

//add detail layer
	
	Volume -= NoiseDetail * _NoiseDetailRange;
	Volume *= 1 + _NoiseDetailRange;
	Volume *= NoiseDensity;

//	return Volume;
	//Volume = Threshold(Volume, NoiseAtten, threshold);
	return saturate(Volume);
}

//http://flafla2.github.io/2016/10/01/raymarching.html
float3 calcNormal(in float3 pos , half DistanceFade)
{
	// epsilon - used to approximate dx when taking the derivative
	const float2 eps = float2(NormalDistance, 0.0);

	// The idea here is to find the "gradient" of the distance field at pos
	// Remember, the distance field is not boolean - even if you are inside an object
	// the number is negative, so this calculation still works.
	// Essentially you are approximating the derivative of the distance field at this point.
	float3 nor = float3(
		noise(pos + eps.xyy, DistanceFade).x - noise(pos - eps.xyy, DistanceFade).x,
		noise(pos + eps.yxy, DistanceFade).x - noise(pos - eps.yxy, DistanceFade).x,
		noise(pos + eps.yyx, DistanceFade).x - noise(pos - eps.yyx, DistanceFade).x);
	return normalize(nor);
}
#endif


#if _FOG_VOLUME_NOISE && _SHADE

half ShadowGrad(half ShadowThreshold, half SampleDiff)
{
	return saturate(ShadowThreshold / (ShadowThreshold - SampleDiff));
}
half ShadowShift = .05;
//#define _SelfShadowSteps 20
float3 Shadow(float3 ShadowCoords, v2f i, half detailDist, half3 LightVector, half NoiseAtten)
{
	
	float ShadowThreshold = noise(ShadowCoords, detailDist);
	float3 LightStep = /*_LightLocalDirection*/LightVector / (float)_SelfShadowSteps;
	
	float accum = 0;
	float3 ShadowCoordsStep = ShadowCoords;
	for (int k = 0; k <= _SelfShadowSteps && NoiseAtten>0; k++, ShadowCoordsStep += LightStep)
	{
		float NoiseSample = 0;
		float SampleDiff = 0;
		if (ShadowThreshold > 0 && accum < 1)
		{
			NoiseSample = noise(ShadowCoordsStep, detailDist);
			SampleDiff = ShadowThreshold - NoiseSample;


			if (SampleDiff <= 0)
			{
				if (ShadowThreshold > 0)
				{
					accum += 1 - ShadowGrad(ShadowThreshold, SampleDiff);
				}
				else
				{
					accum += 1;
				}
			}
			else
				if (ShadowThreshold <= 0)
				{
					accum += ShadowGrad(ShadowThreshold, SampleDiff);
				}
		}
	}
	//return accum;
	return saturate(1-accum);

}
#endif

float4 PointLight(float3 LightPosition, float3 VolumePosition,
	half size, half3 color, half LightAttenuation, v2f i)
{
	//#define ATTEN_METHOD_2		
	half d = distance(LightPosition, VolumePosition) / size;
	float Attenuation = 0;
	half UnityScaleMatch = 5;
	//https://en.wikipedia.org/wiki/Optical_depth
#ifdef ATTEN_METHOD_1			
	Attenuation = exp(-d)*UnityScaleMatch;
#endif

#ifdef ATTEN_METHOD_2
	Attenuation = UnityScaleMatch / (4 * PI*d*d);
#endif

	//Linear
#ifdef ATTEN_METHOD_3
	Attenuation = saturate(1 - d / UnityScaleMatch);
#endif
	return float4(Attenuation * LightAttenuation * color, Attenuation* LightAttenuation);
}

#endif