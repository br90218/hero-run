#ifndef FOG_VOLUME_FRAGMENT_INCLUDED
#define FOG_VOLUME_FRAGMENT_INCLUDED
#define HALF_MAX        65504.0
// Clamp HDR value within a safe range
inline half  SafeHDR(half  c) { return min(c, HALF_MAX); }
inline half2 SafeHDR(half2 c) { return min(c, HALF_MAX); }
inline half3 SafeHDR(half3 c) { return min(c, HALF_MAX); }
inline half4 SafeHDR(half4 c) { return min(c, HALF_MAX); }

float4 frag(v2f i) : COLOR
{
#ifdef RENDER_SCENE_VIEW
	discard;
	//return 0;
#endif
	float3 ViewDir = normalize(i.LocalPos - i.LocalEyePos);
	float tmin = 0.0, tmax = 0.0;
	bool hit = IntersectBox(i.LocalEyePos, ViewDir, _BoxMin.xyz, _BoxMax.xyz, tmin, tmax);
	if (!hit) discard;
	if (tmin < 0.0) tmin = 0.0;
	
	
	 float4 ScreenUVs = UNITY_PROJ_COORD(i.ScreenUVs);
	
	//#ifdef _FOG_LOWRES_RENDERER
	// ScreenUVs.xy /= ScreenUVs.w;

	//	//float Depth = length(DECODE_EYEDEPTH(tex2D(RT_Depth, ScreenUVs.xy).r) / normalize(i.ViewPos).z);
	//	//enconded: 	
	//	float Depth = length(DECODE_EYEDEPTH(DecodeFloatRG(tex2Dproj(RT_Depth, ScreenUVs))) / normalize(i.ViewPos).z);
	//#else
	//	float Depth = length(DECODE_EYEDEPTH(tex2Dproj(_CameraDepthTexture, ScreenUVs).r) / normalize(i.ViewPos).z);
	//#endif
#ifdef _FOG_LOWRES_RENDERER
	 //low res
	 float Depth = /*DecodeFloatRGBA*/(tex2Dproj(RT_Depth, ScreenUVs));
#if  UNITY_REVERSED_Z!=1
	 Depth = 1.0f - Depth;

#endif
	
	
#else
	 //full res
		float Depth = tex2Dproj(_CameraDepthTexture, ScreenUVs).r;
#endif
		Depth = LinearEyeDepth(Depth);
		Depth = length(Depth / normalize(i.ViewPos).z);
		// Depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, ScreenUVs).r);
	//Depth=Depth / 500;
		//Depth = 1000;
		//return float4(Depth, Depth, Depth, 1); break;
	float thickness = min(max(tmin, tmax), Depth) - min(min(tmin, tmax), Depth);
	//return float4(1, 0, 0, 1);
	float Fog = thickness / _Visibility;
	//return float4(1,1,1, Depth/500);
	Fog = 1.0 - exp(-Fog);
	//Fog *= Volume;

	float4 Final = 0;
	float3 Normalized_CameraWorldDir = normalize(i.Wpos - _WorldSpaceCameraPos);

	float3 CameraLocalDir = (i.LocalPos - i.LocalEyePos);
	half InscatteringDistanceClamp = saturate(Depth / InscatteringTransitionWideness - InscatteringStartDistance);
	//  half InscatteringDistanceClamp = saturate((InscatteringTransitionWideness -Depth) / (InscatteringTransitionWideness- InscatteringStartDistance));
#if _FOG_VOLUME_NOISE || _FOG_GRADIENT	

	half jitter = 1;
	#ifdef JITTER
	jitter = remap_tri(nrand(ScreenUVs + frac(_Time.x)));

	jitter = lerp(1, jitter, _jitter);
#endif


	float4 Noise = 1;
	float3 ShadowColor = 0;
	float3 rayStart = i.LocalEyePos + ViewDir * tmin;
	float3 rayStop = i.LocalEyePos + ViewDir * tmax;
	float3 rayDir = rayStop - rayStart;
	float RayLength = length(rayDir);
	Speed *= _Time.x;
	float4 FinalNoise = 0;
	float4 Gradient = 1;
	half Contact = 1;

	half4 PointLightAccum = 0;
	float4 PointLightsFinal = 0;

	
	half DistanceFade = 0;
	half SphereDistance = 0;
	half DetailCascade0 = 0;
	half DetailCascade1 = 0;
	half DetailCascade2 = 0;
	half3 Phase = 0;
	/*half*/ PrimitiveAccum = 1;
	float3 normal = 0;
	half LambertTerm = 1;
	float3 LightTerms = 0;
	half SelfShadows = 1;
	float OpacityTerms = 0;
	half4 debugOutput = 1;
	float DirectLightingShadowStepSize = (1.0 / (float)DirectLightingShadowSteps)*_DirectionalLightingDistance;
	float3 LightVector = _LightLocalDirection;
	//LightVector.xz = LightVector.zx;
	//LightVector.z = -LightVector.z;
	//LightVector *= DirectLightingShadowStepSize;
	float DirectionalLightingAccum = 1;
	float3 DirectionalLighting = 0;
	half3 AmbientTerm = 1;
	half Lambert = 1;
	half absorption = 1;
	half LightShafts = 1;
	half4 VolumeFog = 0;
	half LighsShaftsLightVectorConstrain = VolumeSize.y / VolumeSize.x;
	float3 LightShaftsDir = L;
	LightShaftsDir.xz = LighsShaftsLightVectorConstrain * LightShaftsDir.zx;
	float4 debugIterations = float4(0,0,0,1);
	float t = 0, dt = _RayStep;
//#ifdef JITTER
//	rayStart *= jitter;
//#endif
	float3 r0 = rayStart;
	float3  rd = normalize(rayDir);

#ifdef SAMPLING_METHOD_ViewAligned
	
	float PlaneNdotRay = dot(rd, i.SliceNormal);
	dt = _RayStep / abs(PlaneNdotRay);
	t = dt - fmod(dot(r0, i.SliceNormal), _RayStep) / PlaneNdotRay;
#endif

	float3 VolumeSpaceCoords = 0;

#ifdef JITTER
	t *= jitter;
	dt *= jitter;
#endif
	int VolumeRayDistanceTraveled = 0;
	for (int s = 1; s < STEP_COUNT && RayLength>0; s += 1, t += dt, RayLength -= dt)
	{
		dt *= 1 + s * s * s * _OptimizationFactor;//live fast and die young
		float3 pos = r0 + rd * t;
		VolumeSpaceCoords = pos;
		
		float3 NoiseCoordinates = VolumeSpaceCoords * (_3DNoiseScale * Stretch.rgb);
		DistanceFade = distance(VolumeSpaceCoords, i.LocalEyePos);
		DetailCascade0 = 1 - saturate(DistanceFade / DetailDistance);
		DetailCascade1 = 1 - saturate(DistanceFade / DirectLightingDistance);

#if SHADER_API_GLCORE || SHADER_API_D3D11
#if ATTEN_METHOD_1 || ATTEN_METHOD_2 || ATTEN_METHOD_3
		DetailCascade2 = 1 - saturate(DistanceFade * PointLightingDistance2Camera);
#endif
#endif
		DistanceFade = saturate(DistanceFade / FadeDistance);
		
		DistanceFade = 1 - DistanceFade;
		if (DistanceFade < .001) break;
#if _COLLISION	
		Contact = saturate((Depth - distance(VolumeSpaceCoords, i.LocalEyePos))*_SceneIntersectionSoftness);
		if (Contact < .01)break;
#endif

#ifdef DF

		PrimitiveAccum = 0;
		half3 p = 0;
			for (int k = 0; k < _PrimitiveCount; k++)
			{
				p = mul(_PrimitivesTransform[k], VolumeSpaceCoords - _PrimitivePosition[k]);
				PrimitiveAccum = max(PrimitiveAccum, 1 - (udBox(p, _PrimitiveScale[k] * .5) + Constrain));
			}


			PrimitiveAccum=ContrastF(PrimitiveAccum * _PrimitiveEdgeSoftener, 1);
		

#endif


#if defined(_FOG_GRADIENT)
		half2 GradientCoords = VolumeSpaceCoords.xy / (_BoxMax.xy - _BoxMin.xy) - .5f;
		GradientCoords.y *= 0.95;//correct bottom. must check in the future what's wrong with the uv at the edges
		GradientCoords.y -= 0.04;//3.1.1
		//if(PrimitiveAccum>.99)
		Gradient = tex2Dlod(_Gradient, half4(GradientCoords, 0, 0));

#endif	

		half VerticalGrad = (VolumeSpaceCoords.y / (_BoxMax.y - _BoxMin.y) + 0.5);
#ifdef VOLUME_FOG
		if (OpacityTerms <1)
			VolumeRayDistanceTraveled++;

		float VolumeDepth01 = (float)VolumeRayDistanceTraveled / STEP_COUNT;
		float DistanceCamera2VolumeWalls = length(CameraLocalDir);
		float DistanceCamera2Center = distance(_WorldSpaceCameraPos, _VolumePosition);
		float DistanceCamera2VolumePoints = distance(_WorldSpaceCameraPos, VolumeSpaceCoords);
		float VolumeDepth = min(max(tmin, tmax), DistanceCamera2VolumePoints) - min(min(tmin, tmax), DistanceCamera2VolumePoints);
		float VolumeDensity = DistanceCamera2VolumePoints - DistanceCamera2VolumeWalls;
		VolumeFog = saturate(1 - exp( -VolumeDepth / _Visibility*5));
		//VolumeFog= saturate(1 - exp(-VolumeRayDistanceTraveled / _Visibility));
		
		
		VolumeFog.a *= Contact/**Gradient.a*/;// aquí hay que decidir si el gradiente se lo come o no
	
#endif
								 
		NoiseAtten = gain;
		NoiseAtten *= DistanceFade;
		NoiseAtten *= Contact;


#ifdef HEIGHT_GRAD		
		half heightGradient = HeightGradient(VerticalGrad, GradMin, GradMax);
		NoiseAtten *= heightGradient;
#endif
#if SPHERICAL_FADE
		SphereDistance = 1 - saturate(length(VolumeSpaceCoords) / SphericalFadeDistance);
		NoiseAtten *= SphereDistance;

#endif
		




		//TEXTURE SAMPLERS//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if _FOG_VOLUME_NOISE || _FOG_GRADIENT


#if Twirl_X || Twirl_Y || Twirl_Z
		float3 rotationDegree = length(NoiseCoordinates) * _Vortex + _Rotation + _RotationSpeed * _Time.x;

		NoiseCoordinates = rotate(NoiseCoordinates , rotationDegree);
#endif						



		if (Contact > 0 && NoiseAtten >0 && PrimitiveAccum>_PrimitiveCutout)
		{
#if _FOG_VOLUME_NOISE && !_FOG_GRADIENT
			Noise = noise(NoiseCoordinates, DetailCascade0);
#endif
#if !_FOG_VOLUME_NOISE && _FOG_GRADIENT
			Gradient.a *= gain;
			Noise = Gradient;
#endif

#if _FOG_VOLUME_NOISE && _FOG_GRADIENT
			Noise = noise(NoiseCoordinates, DetailCascade0) * Gradient;
#endif
//#ifdef DF
//			//PrimitiveAccum = saturate(PrimitiveAccum *20);
//		
//			Noise.a *= PrimitiveAccum*PrimitiveAccum *PrimitiveAccum*PrimitiveAccum;
//			NoiseAtten *= PrimitiveAccum;
//#endif
			if (Noise.a>0)
				Noise *= DistanceFade;


		}
		else
		{
			Noise = 0;
			Gradient.a = 0;
		}
		half absorptionFactor = lerp(1, 200, Absorption);
#ifdef ABSORPTION

		half d = Noise.a;//si se multiplica aquí añade cotraste

		//half absorptionFactor = lerp(1, 20, Absorption);
	

		half Beers = exp(-d* absorptionFactor)* absorptionFactor;//la última multiplicación da contraste
		half Powder = 1 - exp(-d * 2);
		absorption = lerp(1, saturate(Beers*Powder), Absorption);
#ifdef HEIGHT_GRAD
		half HeightGradientAtten = 1 - heightGradient;
		HeightGradientAtten = 1.0 - exp(-HeightGradientAtten);
		absorption *= lerp(1,HeightGradientAtten, HeightAbsorption);
#endif
		AmbientTerm = absorption;


#else				
		AmbientTerm = 1;
#endif
		AmbientTerm.rgb *= _AmbientColor.rgb/** Noise.a*/*NoiseAtten;
#endif
#if _SHADE
		if (Noise.a > 0 && NoiseAtten>0)
			SelfShadows = Shadow(NoiseCoordinates, i, DetailCascade0, LightVector* ShadowShift, NoiseAtten);
		
#endif				
#if DIRECTIONAL_LIGHTING
		float DirectionalLightingSample = 0;
	//TODO	if (LightShafts > 0.1)

		if (NoiseAtten>0 && Noise.a>0)
		{
			float3 DirectionalLightingSamplingPosition = NoiseCoordinates;
			for (int s = 0; s < DirectLightingShadowSteps; s++)
			{
				DirectionalLightingSamplingPosition += LightVector*DirectLightingShadowStepSize;
				DirectionalLightingSample = noise(DirectionalLightingSamplingPosition, DetailCascade0).r ;
				DirectionalLightingAccum += DirectionalLightingSample/* / DirectLightingShadowSteps*/;
			}
			DirectionalLighting = DirectionalLightingAccum;
		}
		DirectionalLighting *= Noise.a;
#endif
	
#if defined (_INSCATTERING)
	
#if ABSORPTION 
		//lets diffuse LambertTerm according to density/absorption

		//remap absorption greyscale to -1 1 range to affect anisotropy according to media density
		////////					multiply ranges of [0, 1]
		half t = (1 - Noise.a) * (InscatteringShape*0.5 + 0.5);
		//get back to [-1, 1]
		t = t * 2 - 1;

		InscatteringShape = lerp(InscatteringShape, (t), Absorption);


		//            #if ABSORPTION && VOLUME_FOG
		//						  InscatteringShape = lerp(InscatteringShape*absorption, InscatteringShape, Noise.a);
		//            #endif


#endif
		half HG = Henyey(Normalized_CameraWorldDir, L, InscatteringShape);
		HG *= InscatteringDistanceClamp * Contact;
		Phase = /*_LightColor**/_InscatteringColor.rgb * _InscatteringIntensity * HG * Gradient.xyz;
		
	

//#ifdef VOLUME_FOG
//		//Phase *= VolumeFog.rgb;//WIP
//		
//#else
	Phase *= absorption /** Noise.a*/;
//#endif

#endif			
#if SHADER_API_GLCORE || SHADER_API_D3D11
#if ATTEN_METHOD_1 || ATTEN_METHOD_2 || ATTEN_METHOD_3
		if (NoiseAtten > 0 /*&& VolumeFog.a>0*/ && DetailCascade2>0)
		{
			for (int k = 0; k < _LightsCount; k++)
			{
				half PointLightRange = 1 - (length(_PointLightPositions[k].xyz- VolumeSpaceCoords) * PointLightingDistance);//even calculating this range is too expensive :/
				if (PointLightRange > .99) {
					PointLightAccum +=
						(Noise.a + VolumeFog.a)*
						PointLight(_PointLightPositions[k].xyz, VolumeSpaceCoords, _PointLightsRange[k],
							_PointLightColors[k], _PointLightsIntensity[k], i);
					
				}
			

			}
			half atten = saturate(PointLightAccum.a);
			if (atten > 0)
				PointLightsFinal.rgb = PointLightsFinal.rgb + PointLightAccum.rgb * (1.0 - FinalNoise.a);
		}


#endif
#endif
#ifdef HALO
		half LdotV = saturate(dot(L, Normalized_CameraWorldDir));
		LdotV = pow(LdotV, _HaloRadius);
		LdotV = 1 - exp(-LdotV);
		LdotV = ContrastF(LdotV, _HaloWidth);//franja

		LdotV = saturate(LdotV);
		LdotV -= .5;
	
//#ifdef ABSORPTION
//		half HaloDensityTerm = absorption;
//#else
		half HaloDensityTerm =  Noise.a;
//#endif
		half mip = saturate(Noise.a * 12) * _HaloAbsorption * (1 - HaloDensityTerm);
		half4 Halo = tex2Dlod(_LightHaloTexture, float4(0, LdotV, 0, mip));

		if (LdotV >0)
		{
			Halo.g = tex2Dlod(_LightHaloTexture, float4(0, LdotV * 1.1, 0, mip)/**_HaloOpticalDispersion*/).r;
			Halo.b = tex2Dlod(_LightHaloTexture, float4(0, LdotV * 1.2, 0, mip)/**_HaloOpticalDispersion*/).r;
			//Halo = LdotV;
			Halo.rgb *= Halo.a;
			Halo.rgb *= HaloDensityTerm;
			Halo.rgb *= 1-mip/12;
			Halo.rgb *=_HaloIntensity;
			//Halo.rgb *= absorptionFactor;
			Halo.rgb *=1- HaloDensityTerm;
			Halo.rgb *= _LightColor.rgb;
			Halo.rgb *= LdotV;
			

		}
		else
			Halo = 0;

#endif

		//Blend
		/*	if (NoiseAtten > 0)
		{*/
		OpacityTerms = Noise.a*Contact;

		//Shadow Color
		DirectionalLighting /= LightExtinctionColor.rgb/** _LightColor.rgb*/;
		half3 LightTerms =  exp(-DirectionalLighting* DirectLightingShadowDensity) *  OpacityTerms;
		Phase *= LightTerms;//atten with shadowing
		
		LightTerms *= lerp(_AmbientColor.rgb, 1, absorption);


#ifdef VOLUME_SHADOWS

		half4 LightMapCoords = float4(VolumeSpaceCoords.zx / (_BoxMax.zx - _BoxMin.zx) - .5f, 0, 0);

		LightMapCoords.xy = LightMapCoords.yx;

#ifndef LIGHT_ATTACHED
		LightMapCoords.xy += LightShaftsDir.zx*(1 - VerticalGrad);
#endif
		LightMapCoords.x = clamp(LightMapCoords.x, -1, 0);
		LightMapCoords.y = clamp(LightMapCoords.y, -1, 0);
		LightShafts = tex2Dlod(LightshaftTex, LightMapCoords).r;
		LightShafts = LightShafts*lerp(50,1,_Cutoff);
		LightShafts = 1 - saturate(LightShafts);
			#if defined (_INSCATTERING)
					Phase *= LightShafts;
			#endif
			#ifdef HALO
					Halo *= LightShafts;
			#endif
#endif

#if _LAMBERT_SHADING	
		if (LightShafts > 0.1)//si estamos en sombra, pos no hagas ná
		{
			//Lambert lighting
			if (Noise.a > 0 && NoiseAtten > 0 && DetailCascade1 > 0)
			{
				normal = calcNormal(NoiseCoordinates, DetailCascade0);

				LambertTerm = max(0, dot(normal, normalize(-L)));

				//kind of half lambert
				LambertTerm = LambertTerm*0.5 + LambertianBias;
				LambertTerm *= LambertTerm;
				Lambert = lerp(1, LambertTerm, Noise.a*DirectLightingAmount*ContrastF(DetailCascade1 * 3, 2));
				Lambert = max(0.0, Lambert);
			}
		}
#endif	
		
#ifdef VOLUME_SHADOWS
		LightTerms *= LightShafts;
#endif

		LightTerms *= Lambert;

#ifdef _SHADE
		LightTerms *= SelfShadows;
		Phase*= SelfShadows;
#endif

#ifdef VOLUME_FOG
		
#ifdef VOLUME_SHADOWS
		LightTerms = lerp(LightTerms, _FogColor.rgb*Gradient.rgb*(LightShafts + /*_AmbientColor.rgb**/_AmbientColor.a), VolumeFog.a);
#else
		LightTerms = lerp(LightTerms, _FogColor.rgb*Gradient.rgb, VolumeFog.a);
#endif
		
#if defined (_VOLUME_FOG_INSCATTERING)
		
		half VolumeFogInscatteringDistanceClamp =saturate((VolumeRayDistanceTraveled - VolumeFogInscatteringStartDistance) / VolumeFogInscatteringTransitionWideness );
		half3 VolumeFogPhase = Henyey(Normalized_CameraWorldDir, L, VolumeFogInscatteringAnisotropy);
		VolumeFogPhase *= Contact;
		VolumeFogPhase *= VolumeFogInscatteringDistanceClamp;
		VolumeFogPhase *=/* _LightColor.rgb **/ VolumeFogInscatteringColor.rgb * VolumeFogInscatteringIntensity * Gradient.xyz;	
		VolumeFogPhase *= saturate(1 - Noise.a * VolumeFogInscatteringIntensity/*pushin' proportionaly to intensity*/);
			half3 FogInscatter = _FogColor.rgb * VolumeFogPhase;

		#ifdef VOLUME_SHADOWS		
				LightTerms = LightTerms + VolumeFog.a * FogInscatter * _FogColor.a * LightShafts;
		#else
				LightTerms = LightTerms + VolumeFog.a * FogInscatter * _FogColor.a;
		#endif
#endif
		OpacityTerms = min(OpacityTerms + VolumeFog.a, 1);
#endif


#if defined(_FOG_GRADIENT)
		LightTerms *= Gradient.rgb;

#endif	
		LightTerms += Phase;
		//Multiply by LambertTerm and color before additive stuff
		LightTerms *= _LightColor.rgb * _Color.rgb;
		LightTerms *= _LightExposure;//new in 3.1.1
		LightTerms += AmbientTerm*Noise.a;//multiplicando para no afectar a la niebla
		

#ifdef HALO

		LightTerms += Halo.rgb;
#endif
		
#ifdef DEBUG
		if (_DebugMode == DEBUG_ITERATIONS)
		{
			debugOutput.rgb = tex2Dlod(_PerformanceLUT, float4(0, (float)s / STEP_COUNT, 0, 0)).rgb;
		}
		if (_DebugMode == DEBUG_INSCATTERING)
		{
			LightTerms = Phase;
		}

		if (_DebugMode == DEBUG_VOLUMETRIC_SHADOWS)
		{
			LightTerms = LightShafts * .05;
		}
#if VOLUME_FOG && _VOLUME_FOG_INSCATTERING
		if (_DebugMode == DEBUG_VOLUME_FOG_INSCATTER_CLAMP)
		{
			LightTerms = OpacityTerms * VolumeFogInscatteringDistanceClamp;
		}
		if (_DebugMode == DEBUG_VOLUME_FOG_PHASE)
		{
			LightTerms = OpacityTerms * FogInscatter;
		}
#endif
		
#endif
	
		FinalNoise.a *= _PushAlpha;
		FinalNoise.a = saturate(FinalNoise.a);
		
		FinalNoise = FinalNoise + float4(LightTerms, OpacityTerms)  * (1.0 - FinalNoise.a);
		//FinalNoise.rgb += Phase;
		if (FinalNoise.a > .999)break;//KILL'EM ALL if its already opaque, don't do anything else
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////LOOP END
#ifdef DEBUG

	if (_DebugMode == DEBUG_INSCATTERING 
		|| _DebugMode == DEBUG_VOLUMETRIC_SHADOWS 
		|| _DebugMode == DEBUG_VOLUME_FOG_INSCATTER_CLAMP
		|| _DebugMode == DEBUG_VOLUME_FOG_PHASE)
	{
		debugOutput = FinalNoise;

	}

	return debugOutput;
#endif
	//return FinalNoise;
	//return float4(Contact, Contact, Contact,1);
	//return float4(NoiseAtten, NoiseAtten, NoiseAtten, 1); ;
	//FinalNoise.rgb *= _Color.rgb;
#if SHADER_API_GLCORE || SHADER_API_D3D11
#if ATTEN_METHOD_1 || ATTEN_METHOD_2 || ATTEN_METHOD_3

	FinalNoise.rgb += PointLightsFinal.rgb;
#endif
#endif
	_Color = FinalNoise;
	_InscatteringColor *= FinalNoise;


#endif				

#if _INSCATTERING && !_FOG_VOLUME_NOISE && !_FOG_GRADIENT

	float Inscattering = Henyey(Normalized_CameraWorldDir, L, InscatteringShape);
	//_InscatteringIntensity *= .05;
	Inscattering *= InscatteringDistanceClamp;
	Final = float4(_Color.rgb + _InscatteringColor.rgb * _InscatteringIntensity * Inscattering, _Color.a);

#else
	Final = _Color;
#endif	

#ifdef ColorAdjust

	
	Final.rgb = lerp(Final.rgb, pow(max((Final.rgb + Offset), 0), 1 / Gamma), Final.a);
	
#if _TONEMAP			
	Final.rgb = ToneMap(Final.rgb, Exposure);
#endif
#endif
#if !_FOG_VOLUME_NOISE && !_FOG_GRADIENT
	Final.a *= (Fog * _Color.a);
#endif
	
	if (IsGammaSpace())
		Final.rgb = pow(Final.rgb, 1 / 2.2);

	Final.a = saturate(Final.a);
	Final.rgb = SafeHDR(Final.rgb);
	clip(Final.a);
	return Final;

}
#endif