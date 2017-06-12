using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEditorInternal;

[CustomEditor(typeof(FogVolume))]

public class FogVolumeEditor : Editor
{
    #region Variables

    //private static bool showLighting
    //{
    //    get { return EditorPrefs.GetBool("showLightingTab", false); }
    //    set { EditorPrefs.SetBool("showLightingTab", value); }
    //}
    //private static bool OtherTAB
    //{
    //    get { return EditorPrefs.GetBool("showOtherTAB", false); }
    //    set { EditorPrefs.SetBool("showOtherTAB", value); }
    //}
    //private static bool showGradient
    //{
    //    get { return EditorPrefs.GetBool("showGradient", false); }
    //    set { EditorPrefs.SetBool("showGradient", value); }
    //}
    //private static bool showPointLights
    //{
    //    get { return EditorPrefs.GetBool("showPointLights", false); }
    //    set { EditorPrefs.SetBool("showPointLights", value); }
    //}

    //private static bool showNoiseProperties
    //{
    //    get { return EditorPrefs.GetBool("showNoiseProperties", false); }
    //    set { EditorPrefs.SetBool("showNoiseProperties", value); }
    //}

    //private static bool showVolumeProperties
    //{
    //    get { return EditorPrefs.GetBool("showVolumeProperties", false); }
    //    set { EditorPrefs.SetBool("showVolumeProperties", value); }
    //}

    //private static bool showColorTab
    //{
    //    get { return EditorPrefs.GetBool("showColorTab", false); }
    //    set { EditorPrefs.SetBool("showColorTab", value); }
    //}
    //private static bool tempShadowCaster
    //{
    //    get { return EditorPrefs.GetBool("tempShadowCaster", false); }
    //    set { EditorPrefs.SetBool("tempShadowCaster", value); }
    //}
    //private static bool showCustomizationOptions
    //{
    //    get { return EditorPrefs.GetBool(_showCustomizationOptions, false); }
    //    set { EditorPrefs.SetBool(_showCustomizationOptions, value); }
    //}

    //private const string _showOtherOptions = "showOtherOptions";
    //private static bool showOtherOptions
    //{
    //    get { return EditorPrefs.GetBool(_showOtherOptions, false); }
    //    set { EditorPrefs.SetBool(_showOtherOptions, value); }
    //}
    FogVolume _target;
    private const string _showCustomizationOptions = "showCustomizationOptions";


    //https://forum.unity3d.com/threads/changing-the-background-color-for-beginhorizontal.66015/#post-430613
    private Texture2D MakeTex(Color col)
    {
        Color[] pix = new Color[1];

        //   for (int i = 0; i < pix.Length; i++)
        pix[0] = col;

        Texture2D result = new Texture2D(1, 1);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    private GUIStyle Theme1, ThemeFooter;
    #endregion

    void OnEnable()
    {
        _target = (FogVolume)target;


        Theme1 = new GUIStyle();
        ThemeFooter = new GUIStyle();
        //  ThemeFooter.normal.background = MakeTex(new Color(.31f, 0.2f, .3f));
        if (EditorGUIUtility.isProSkin)
            ThemeFooter.normal.background = (Texture2D)Resources.Load("RendererInspectorBodyBlack");
        else
            ThemeFooter.normal.background = (Texture2D)Resources.Load("RendererInspectorBodyBright");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        serializedObject.Update();
        GUI.color = Color.white;
        GUILayout.BeginVertical(Theme1);
        //some info about fog type
        if (GUILayout.Button("Info", EditorStyles.toolbarButton))        
            _target.generalInfo = !_target.generalInfo;

        if (_target.generalInfo)
        {
            GUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField("Version 3.1.8");
            EditorGUILayout.LabelField("Release date: April 2017");
            EditorGUILayout.LabelField("Fog type: " + _target._FogType.ToString());

            #region Camera

            if (_target.GameCameraGO)
            {
                EditorGUILayout.LabelField("Assigned camera: " + _target.GameCameraGO.name);
                if (GUILayout.Button("Select "+ _target.GameCameraGO.name, EditorStyles.toolbarButton))
                    Selection.activeGameObject = _target.GameCameraGO;
            }
            else
                GUILayout.Button("No valid camera found", EditorStyles.toolbarButton);
            GUILayout.EndVertical();
            GUILayout.Space(10);
            #endregion

        }

        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(target, "Fog volume parameter");
        #region Basic


        // _target.RenderableInSceneView = EditorGUILayout.Toggle("Render In Scene View", _target.RenderableInSceneView);
        if (_target._FogType == FogVolume.FogType.Textured)
            _target.FogMainColor = EditorGUILayout.ColorField("Light energy", _target.FogMainColor);
        else
            _target.FogMainColor = EditorGUILayout.ColorField("Color", _target.FogMainColor);

        if (!_target.EnableNoise && !_target.EnableGradient || _target.bVolumeFog)
            _target.Visibility = EditorGUILayout.FloatField("Visibility", _target.Visibility);
        _target.fogVolumeScale = EditorGUILayout.Vector3Field("Size", _target.fogVolumeScale);
        if (_target._FogType == FogVolume.FogType.Textured)
        {
            _target.bVolumeFog = EditorGUILayout.Toggle("Volume Fog", _target.bVolumeFog);
            if (_target.bVolumeFog)
                _target._FogColor = EditorGUILayout.ColorField("Volume Fog Color", _target._FogColor);
        }
        _target._BlendMode = (FogVolumeRenderer.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _target._BlendMode);
        #endregion

        #region lighting
        //GUILayout.BeginHorizontal("toolbarbutton");
        //EditorGUILayout.LabelField("Lighting");
        //showLighting = EditorGUILayout.Toggle("Show", showLighting);
        //GUILayout.EndHorizontal();
        //if (_target.EnableNoise /*|| _target.EnableGradient*/)
        if (GUILayout.Button("Lighting", EditorStyles.toolbarButton))
            _target.showLighting = !_target.showLighting;

        if (_target.showLighting)
        {
            if (_target._FogType == FogVolume.FogType.Textured)
            {
                GUILayout.BeginVertical("box");

                _target._AmbientColor = EditorGUILayout.ColorField("Ambient", _target._AmbientColor);
                _target.Absorption = EditorGUILayout.Slider("Absorption", _target.Absorption, 0, 1);
                if (_target.useHeightGradient)
                {
                    _target.HeightAbsorption = EditorGUILayout.Slider("Height Absorption", _target.HeightAbsorption, 0, 1);
                }
                GUILayout.EndVertical();
                // if( GUILayout.Button(_target.bAbsorption.ToString()))
                //   _target.bAbsorption = !_target.bAbsorption;
                // else
                // _target.bAbsorption = false;

                //  
                // Debug.Log(_target.bAbsorption);
                if (_target.Absorption == 0)
                    _target.bAbsorption = false;
                else
                    _target.bAbsorption = true;

            }
            _target.Sun = (Light)EditorGUILayout.ObjectField("Sun Light", _target.Sun, typeof(Light), true);



            if (_target.Sun != null)
            {
                if (_target._FogType == FogVolume.FogType.Textured)
                    _target._LightExposure = EditorGUILayout.Slider("Light Exposure", _target._LightExposure, 1, 5);
                if (_target.EnableNoise)
                {
                    GUILayout.BeginVertical("box");
                    _target.Lambert = EditorGUILayout.Toggle("Lambertian", _target.Lambert);
                    if (_target.Lambert)
                    {
                        _target.DirectLightingAmount = EditorGUILayout.Slider("Amount", _target.DirectLightingAmount, .01f, 10f);
                        _target.LambertianBias = EditorGUILayout.Slider("Lambertian Bias", _target.LambertianBias, .5f, 1);
                        _target.NormalDistance = EditorGUILayout.Slider("Normal detail", _target.NormalDistance, .01f, .0001f);
                        _target.DirectLightingDistance = EditorGUILayout.FloatField("Distance", _target.DirectLightingDistance);
                    }
                    GUILayout.EndVertical();
                }
                #region VolumeFogInscattering
                if (_target._FogType == FogVolume.FogType.Textured)
                    if (_target.bVolumeFog)
                    {
                        GUILayout.BeginVertical("box");
                        _target.VolumeFogInscattering = EditorGUILayout.Toggle("Volume Fog Inscattering", _target.VolumeFogInscattering);
                        if (_target.VolumeFogInscattering && _target.bVolumeFog)
                        {
                            _target.VolumeFogInscatteringColor = EditorGUILayout.ColorField("Color", _target.VolumeFogInscatteringColor);
                            _target.VolumeFogInscatteringAnisotropy = EditorGUILayout.Slider("Anisotropy", _target.VolumeFogInscatteringAnisotropy, -1, 1);
                            _target.VolumeFogInscatteringIntensity = EditorGUILayout.Slider("Intensity", _target.VolumeFogInscatteringIntensity, 0, 1);
                            _target.VolumeFogInscatteringStartDistance = EditorGUILayout.FloatField("Start Distance", _target.VolumeFogInscatteringStartDistance);
                            _target.VolumeFogInscatteringTransitionWideness = EditorGUILayout.FloatField("Transition Wideness", _target.VolumeFogInscatteringTransitionWideness);
                        }
                        GUILayout.EndVertical();
                    }

                #endregion

                GUILayout.BeginVertical("box");
                _target.EnableInscattering = EditorGUILayout.Toggle("Inscattering", _target.EnableInscattering);
                if (_target.EnableInscattering)
                {

                    _target.InscatteringColor = EditorGUILayout.ColorField("Color", _target.InscatteringColor);
                    _target.InscatteringShape = EditorGUILayout.Slider("Anisotropy", _target.InscatteringShape, -1, 1);
                    _target.InscatteringIntensity = EditorGUILayout.Slider("Intensity", _target.InscatteringIntensity, 0, 1);
                    _target.InscatteringStartDistance = EditorGUILayout.FloatField("Start Distance", _target.InscatteringStartDistance);
                    _target.InscatteringTransitionWideness = EditorGUILayout.FloatField("Transition Wideness", _target.InscatteringTransitionWideness);


                }
                GUILayout.EndVertical();



                if (_target.EnableNoise && _target._NoiseVolume != null)
                {
                    GUILayout.BeginVertical("box");

                    _target._DirectionalLighting = EditorGUILayout.Toggle("Directional Lighting", _target._DirectionalLighting);

                    if (_target._DirectionalLighting)
                    {

                        _target.LightExtinctionColor = EditorGUILayout.ColorField("Extinction Color", _target.LightExtinctionColor);
                        _target._DirectionalLightingDistance = EditorGUILayout.Slider("Distance", _target._DirectionalLightingDistance, 0, .05f);
                        _target.DirectLightingShadowDensity = EditorGUILayout.Slider("Density", _target.DirectLightingShadowDensity, 0, 15);
                        _target.DirectLightingShadowSteps = EditorGUILayout.IntSlider("Iterations", _target.DirectLightingShadowSteps, 1, 5);

                    }
                    GUILayout.EndVertical();
                }
            }



            if (_target.EnableNoise && _target.Sun)
            {


                GUILayout.BeginVertical("box");
                _target._ShadeNoise = EditorGUILayout.Toggle("Self shadow", _target._ShadeNoise);
                if (_target._ShadeNoise)
                {

                    // _target.Shade = EditorGUILayout.Slider("Shadow intensity", _target.Shade, 0, 1);
                    _target.ShadowShift = EditorGUILayout.Slider("Shadow distance", _target.ShadowShift, .0f, .1f);
                    _target._SelfShadowSteps = EditorGUILayout.IntSlider("Iterations", _target._SelfShadowSteps, 1, 20);
                }
                GUILayout.EndVertical();
            }

            // 
            if (_target._FogType == FogVolume.FogType.Textured)
            {

                if (_target.FogRenderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On ||
                    _target.FogRenderer.receiveShadows == true)
                {
                    GUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Opacity shadow map", EditorStyles.boldLabel);
                    // _target.CastShadows = EditorGUILayout.Toggle("Cast Shadows", _target.CastShadows);//lets control it trough the renderer
                    if (_target.Sun && _target.CastShadows)//casi mejor que un droplist
                    {
                        //tempShadowCaster = _target.CastShadows;
                        _target.ShadowCameraPosition = EditorGUILayout.IntField("Vertical Offset", _target.ShadowCameraPosition);
                        _target.SunAttached = EditorGUILayout.Toggle("Attach light", _target.SunAttached);
                        _target.ShadowColor = EditorGUILayout.ColorField("Shadow Color", _target.ShadowColor);
                        _target._ShadowCamera.textureSize = (ShadowCamera.TextureSize)EditorGUILayout.EnumPopup("Resolution", _target._ShadowCamera.textureSize);
                        GUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Convolution", EditorStyles.boldLabel);

                        _target._ShadowCamera.iterations = EditorGUILayout.IntSlider("Iterations", _target._ShadowCamera.iterations, 0, 5);
                        if (_target._ShadowCamera.iterations > 0)
                            _target._ShadowCamera.Downsampling = EditorGUILayout.IntSlider("Downsampling", _target._ShadowCamera.Downsampling, 0, 5);
                        if (_target._ShadowCamera.iterations > 1)
                            _target._ShadowCamera.blurSpread = EditorGUILayout.Slider("Radius", _target._ShadowCamera.blurSpread, 0, 1);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        if (_target.FogRenderer.receiveShadows)
                            _target.ShadowCaster = (FogVolume)EditorGUILayout.ObjectField("Shadowmap coming from:", _target.ShadowCaster, typeof(FogVolume), true);


                        if (_target.ShadowCaster != null)
                            _target.CastShadows = false;

                        // tempShadowCaster = _target.CastShadows;
                    }
                    if (_target.CastShadows == false && _target.FogRenderer.receiveShadows)
                    {
                        _target.UseConvolvedLightshafts = EditorGUILayout.Toggle("Use convolved lightshaft", _target.UseConvolvedLightshafts);
                        _target.ShadowCutoff = EditorGUILayout.Slider("Shadow Cutoff", _target.ShadowCutoff, 0.001f, 1);
                    }
                    GUILayout.EndVertical();
                }
                //_target.RT_Opacity = (RenderTexture)EditorGUILayout.ObjectField("LightMapRT", _target.RT_Opacity, typeof(RenderTexture), false);
                //_target._LightMapTex = (Texture2D)EditorGUILayout.ObjectField("LightMapTex", _target._LightMapTex, typeof(Texture2D), false);

            }
            else
                _target.CastShadows = false;

            if (_target.Sun && _target.EnableNoise)
            {
                GUILayout.BeginVertical("box");

                _target.LightHalo = EditorGUILayout.Toggle("Halo", _target.LightHalo);
                if (_target.LightHalo && _target.EnableNoise)
                {
                    _target._LightHaloTexture = (Texture2D)EditorGUILayout.ObjectField("Halo texture", _target._LightHaloTexture, typeof(Texture2D), true);
                    _target._HaloWidth = EditorGUILayout.Slider("Width", _target._HaloWidth, 0, 1);

                    _target._HaloRadius = EditorGUILayout.Slider("Radius", _target._HaloRadius, 0, 1);
                    _target._HaloAbsorption = EditorGUILayout.Slider("Absorption", _target._HaloAbsorption, 0, 1);
                    // _target._HaloOpticalDispersion = EditorGUILayout.Slider("OpticalDispersion", _target._HaloOpticalDispersion, 0, 8);
                    _target._HaloIntensity = EditorGUILayout.Slider("Intensity", _target._HaloIntensity, 0, 20);
                }
                GUILayout.EndVertical();
            }//TODO. Halo should be always available
        }

        #endregion

        #region PointLights
        string PointLightTittle = "";
        if (SystemInfo.graphicsShaderLevel > 30)
            PointLightTittle = "Point lights";
        else
            PointLightTittle = "Point lights not available on this platform ";// + SystemInfo.graphicsShaderLevel;

        if (_target._FogType == FogVolume.FogType.Textured)
            if (GUILayout.Button(PointLightTittle, EditorStyles.toolbarButton))
                _target.showPointLights = !_target.showPointLights;

        if (_target.showPointLights && SystemInfo.graphicsShaderLevel > 30)
            _target.PointLightsActive = EditorGUILayout.Toggle("Enable", _target.PointLightsActive);
        if (_target.showPointLights && _target.PointLightsActive)
            if (_target._FogType == FogVolume.FogType.Textured)
            {
                _target.PointLightsRealTimeUpdate = EditorGUILayout.Toggle("Real-time search", _target.PointLightsRealTimeUpdate);
                _target.PointLightBoxCheck = EditorGUILayout.Toggle("Inside box only", _target.PointLightBoxCheck);
                //GUILayout.BeginHorizon=tal("toolbarbutton");
                //EditorGUILayout.LabelField("Point lights");

                //GUILayout.EndHorizontal();
                _target._LightScatterMethod = (FogVolume.LightScatterMethod)EditorGUILayout.EnumPopup("Attenuation Method", _target._LightScatterMethod);
                _target.PointLightsIntensity = EditorGUILayout.Slider("Intensity", _target.PointLightsIntensity, 0, 10);
                _target.PointLightingDistance = Mathf.Max(1, _target.PointLightingDistance);
                _target.PointLightingDistance = EditorGUILayout.FloatField("Range clamp", _target.PointLightingDistance);
                _target.PointLightingDistance2Camera = Mathf.Max(1, _target.PointLightingDistance2Camera);

                _target.PointLightingDistance2Camera = EditorGUILayout.FloatField("Draw distance", _target.PointLightingDistance2Camera);
                //_target.PointLightScreenMargin = EditorGUILayout.Slider("Discard margin", _target.PointLightScreenMargin, 0, 5);deprecated
                _target.PointLightScreenMargin = 0;
                if (!_target.PointLightsRealTimeUpdate)
                {
                    var FogLights = serializedObject.FindProperty("FogLights");
                    GUILayout.BeginVertical("box");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(FogLights, new GUIContent("Editable array"), true);
                    EditorGUI.indentLevel--;
                    GUILayout.EndVertical();
                }

                var PointLightsList = serializedObject.FindProperty("PointLightsList");
                GUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(PointLightsList, new GUIContent("Currently computed:"), true);
                EditorGUI.indentLevel--;
                GUILayout.EndVertical();
                EditorGUILayout.HelpBox("Rendering " + _target.PointLightsList.Count.ToString() + " point lights", MessageType.None);
            }

        #endregion

        #region ColorSettings
        //GUILayout.BeginHorizontal("toolbarbutton");

        //_target.ColorAdjust = EditorGUILayout.Toggle("Color Settings", _target.ColorAdjust);

        //showColorTab = EditorGUILayout.Toggle("     Show            \t     ", showColorTab);
        //GUILayout.EndHorizontal();
        if (GUILayout.Button("Color management", EditorStyles.toolbarButton))
            _target.showColorTab = !_target.showColorTab;

        if (/*_target.ColorAdjust && */_target.showColorTab)
        {
            _target.ColorAdjust = EditorGUILayout.Toggle("Active", _target.ColorAdjust);
            
            _target.Offset = EditorGUILayout.Slider("Offset", _target.Offset, -.5f, .5f);
            _target.Gamma = EditorGUILayout.Slider("Gamma", _target.Gamma, .01f, 3);

            if (_target.ColorAdjust)
                _target.Tonemap = EditorGUILayout.Toggle("Tonemap", _target.Tonemap);
            if (_target.Tonemap && _target.ColorAdjust)
                _target.Exposure = EditorGUILayout.Slider("Exposure", _target.Exposure, 2.5f, 5);
        }
        #endregion

        #region Renderer

        if (_target._FogType == FogVolume.FogType.Textured)
        {
            // GUILayout.BeginHorizontal("toolbarbutton");
            // EditorGUILayout.LabelField("Volume properties");
            //showVolumeProperties = EditorGUILayout.Toggle("Show", showVolumeProperties);
            // GUILayout.EndHorizontal();

            if (GUILayout.Button("Renderer", EditorStyles.toolbarButton))
                _target.showVolumeProperties = !_target.showVolumeProperties;

            if (_target.showVolumeProperties)
            {



                _target.NoiseIntensity = EditorGUILayout.Slider("Intensity", _target.NoiseIntensity, 0, 1);
                if (_target.EnableNoise)
                {
                    _target.NoiseContrast = EditorGUILayout.Slider("Contrast", _target.NoiseContrast, 0, 20);
                }

                GUILayout.BeginVertical("box");
                _target.SceneCollision = EditorGUILayout.Toggle("Scene Collision", _target.SceneCollision);
                if (_target.SceneCollision)
                    _target._SceneIntersectionSoftness = EditorGUILayout.Slider("Softness", _target._SceneIntersectionSoftness, 50, .01f);
                GUILayout.EndVertical();

                _target._jitter = EditorGUILayout.Slider("Jitter", _target._jitter, 0, .1f);

                _target._SamplingMethod = (FogVolume.SamplingMethod)EditorGUILayout.EnumPopup("Sampling Method", _target._SamplingMethod);
                _target.Iterations = EditorGUILayout.IntSlider("Max Iterations", _target.Iterations, 10, 1000);
                _target.IterationStep = EditorGUILayout.FloatField("Iteration step size", _target.IterationStep);
                _target.FadeDistance = EditorGUILayout.FloatField("Draw distance", _target.FadeDistance);
                _target._OptimizationFactor = EditorGUILayout.Slider("Optimization Factor", _target._OptimizationFactor, 0, .000005f);


                GUILayout.BeginVertical("box");
                _target.useHeightGradient = EditorGUILayout.Toggle("Height Gradient", _target.useHeightGradient);
                if (_target.useHeightGradient)
                {
                    _target.GradMin = EditorGUILayout.Slider("Grad Min", _target.GradMin, -1, 2);
                    _target.GradMax = EditorGUILayout.Slider("Grad Max", _target.GradMax, -1, 1);

                }
                EditorGUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                _target.bSphericalFade = EditorGUILayout.Toggle("Radius Fade", _target.bSphericalFade);
                if (_target.bSphericalFade)
                {


                    _target.SphericalFadeDistance = EditorGUILayout.FloatField("Distance", _target.SphericalFadeDistance);
                }
                EditorGUILayout.EndVertical();

                _target._DebugMode = (FogVolume.DebugMode)EditorGUILayout.EnumPopup("View mode: ", _target._DebugMode);

                //Primitives
                GUILayout.BeginHorizontal("toolbarbutton");
                _target.EnableDistanceFields = EditorGUILayout.Toggle("Enable Primitives", _target.EnableDistanceFields);
                if (_target.EnableDistanceFields)
                    EditorGUILayout.LabelField(_target.PrimitivesList.Count + " primitives in use");
                else
                    EditorGUILayout.LabelField("0 primitives in use");
                GUILayout.EndHorizontal();

                if (_target.EnableDistanceFields)
                {
                    GUILayout.BeginVertical("box");
                    _target.ShowPrimitives = EditorGUILayout.Toggle("Show primitives", _target.ShowPrimitives);
                    _target.PrimitivesRealTimeUpdate = EditorGUILayout.Toggle("Real-time search", _target.PrimitivesRealTimeUpdate);
                    EditorGUILayout.EndVertical();
                    _target.Constrain = EditorGUILayout.Slider("Size", _target.Constrain, 0, -15);
                    _target._PrimitiveEdgeSoftener = EditorGUILayout.Slider("Softness", _target._PrimitiveEdgeSoftener, 1, 10);
                    _target._PrimitiveCutout = EditorGUILayout.Slider("Cutout", _target._PrimitiveCutout, 0, .99999f);

                    var PrimitivesList = serializedObject.FindProperty("PrimitivesList");

                    EditorGUILayout.PropertyField(PrimitivesList, true);


                }

            }
        }
        #endregion

        #region Noise
        if (GUILayout.Button("Noise", EditorStyles.toolbarButton))
            _target.showNoiseProperties = !_target.showNoiseProperties;
        // GUILayout.BeginHorizontal("toolbarbutton");
        //EditorGUILayout.LabelField("");

        // showNoiseProperties = EditorGUILayout.Toggle("      Show", showNoiseProperties);
        //GUILayout.EndHorizontal();

        if (/*_target.EnableNoise && */_target.showNoiseProperties)
        {

            _target.EnableNoise = EditorGUILayout.Toggle("Active", _target.EnableNoise);
            if (_target.EnableNoise)
            {
                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Common", EditorStyles.boldLabel);

                // _target.CastShadows = tempShadowCaster;
                _target._3DNoiseScale = EditorGUILayout.FloatField("Scale", _target._3DNoiseScale);
                _target.NoiseDensity = EditorGUILayout.Slider("Density", _target.NoiseDensity, 0, 5);
                //_target.Procedural = EditorGUILayout.Toggle("Procedural", _target.Procedural);

                _target.Octaves = EditorGUILayout.IntSlider("Octaves", _target.Octaves, 1, 2);

                //
                #region Coordinates&Deformers
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Coordinates", EditorStyles.boldLabel);

                _target.Speed = EditorGUILayout.Vector3Field("Scroll", _target.Speed);
                _target.Stretch = EditorGUILayout.Vector3Field("Stretch", _target.Stretch);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Deformers", EditorStyles.boldLabel);


                _target.Vortex = EditorGUILayout.Slider("Swirl", _target.Vortex, 0, 3);
                if (_target.Vortex > 0)
                {
                    _target.RotationSpeed = EditorGUILayout.Slider("Rotation Speed", _target.RotationSpeed, 0, 10);
                    _target.rotation = EditorGUILayout.Slider("Rotation", _target.rotation, 0, 360);
                    _target._VortexAxis = (FogVolume.VortexAxis)EditorGUILayout.EnumPopup("Axis", _target._VortexAxis);
                }
                GUILayout.EndVertical();

                //
                GUILayout.EndVertical();
                //  GUILayout.EndVertical();
                #endregion
                GUILayout.BeginVertical("Box");

                EditorGUILayout.LabelField("Base layer", EditorStyles.boldLabel);
                _target.Coverage = EditorGUILayout.Slider("Coverage", _target.Coverage, 0, 5);
                _target.BaseTiling = EditorGUILayout.FloatField("Base Tiling", _target.BaseTiling);
                _target._BaseRelativeSpeed = EditorGUILayout.Slider("Speed", _target._BaseRelativeSpeed, 0, 4);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Detail layer", EditorStyles.boldLabel);
                _target.DetailTiling = EditorGUILayout.FloatField("Tiling", _target.DetailTiling);
                _target._DetailRelativeSpeed = EditorGUILayout.Slider("Speed", _target._DetailRelativeSpeed, 0, 20);
                _target.DetailDistance = EditorGUILayout.FloatField("Draw Distance", _target.DetailDistance);
                if (_target.DetailDistance > 0)
                {
                    _target._DetailMaskingThreshold = EditorGUILayout.Slider("Masking Threshold", _target._DetailMaskingThreshold, 1, 50);
                    _target.DetailSamplingBaseOpacityLimit = EditorGUILayout.Slider("Cutoff", _target.DetailSamplingBaseOpacityLimit, 0, 1);

                    _target._NoiseDetailRange = EditorGUILayout.Slider("Edge Inner cut", _target._NoiseDetailRange, 0, 1);
                    if (_target.Octaves > 1)
                        _target._Curl = EditorGUILayout.Slider("Curl", _target._Curl, 0, 1);
                }
                GUILayout.EndVertical();





                //_target.noise2D= (Texture2D)EditorGUILayout.ObjectField("noise2D", _target.noise2D, typeof(Texture2D), false);





            }
        }

        #endregion

        #region Gradient
        if (GUILayout.Button("Gradient", EditorStyles.toolbarButton))
            _target.showGradient = !_target.showGradient;

        if (_target.showGradient)
        {
            _target.EnableGradient = EditorGUILayout.Toggle("Active", _target.EnableGradient);

            if (_target.EnableGradient)
                _target.Gradient = (Texture2D)EditorGUILayout.ObjectField("", _target.Gradient, typeof(Texture2D), true);
        }
        #endregion



        #region Footer
        GUILayout.EndVertical();//termina estilo anterior

        EditorGUI.indentLevel++;
        if (GUILayout.Button("Other", EditorStyles.toolbarButton))
            _target.OtherTAB = !_target.OtherTAB;

        GUILayout.BeginVertical(ThemeFooter);

        if (_target.OtherTAB)
        {
            GUILayout.Space(10);
            var OtherRect = GUILayoutUtility.GetRect(new GUIContent(""), GUIStyle.none);
            _target.showOtherOptions = EditorGUI.Foldout(OtherRect, _target.showOtherOptions, "Rendering options");
            if (_target.showOtherOptions)
            {
                //EditorGUI.indentLevel++;


                _target.DrawOrder = EditorGUILayout.IntField("DrawOrder", _target.DrawOrder);
                _target._PushAlpha = EditorGUILayout.Slider("Push Alpha", _target._PushAlpha, 1, 1.025f);
                _target._ztest = (UnityEngine.Rendering.CompareFunction)EditorGUILayout.EnumPopup("ZTest ", _target._ztest);
                if (_target.GameCameraGO != null /*&& _target._FogType == FogVolume.FogType.Textured*/)
                    if (_target.GameCameraGO.GetComponent<FogVolumeRenderer>()._Downsample > 0 && _target.GameCameraGO.GetComponent<FogVolumeRenderer>())
                        _target.CreateSurrogate = EditorGUILayout.Toggle("Create Surrogate mesh", _target.CreateSurrogate);
                
            }
            if (EditorGUIUtility.isProSkin)
            {
                GUILayout.Space(10);
                var CustomizationOptionsRect = GUILayoutUtility.GetRect(new GUIContent(""), GUIStyle.none);
                _target.showCustomizationOptions = EditorGUI.Foldout(CustomizationOptionsRect, _target.showCustomizationOptions, "Customize look");
                if (_target.showCustomizationOptions)
                {
                    // EditorGUI.indentLevel++;
                    // _target._InspectorBackground = (Texture2D)EditorGUILayout.ObjectField("Background", _target._InspectorBackground, typeof(Texture2D), false);
                    if (_target._InspectorBackground.Length > 0)
                        _target._InspectorBackgroundIndex = EditorGUILayout.IntSlider("Inspector background", _target._InspectorBackgroundIndex, 0, _target._InspectorBackground.Length - 1);
                    _target.HideWireframe = EditorGUILayout.Toggle("Scene view wireframe", _target.HideWireframe);
                    // EditorGUI.indentLevel--;
                }

                GUILayout.Space(10);
            }
        }
        if (_target._InspectorBackground.Length > 0)
            if (_target._InspectorBackground[_target._InspectorBackgroundIndex] != null && EditorGUIUtility.isProSkin)
                Theme1.normal.background = _target._InspectorBackground[_target._InspectorBackgroundIndex];
        GUILayout.EndVertical();//end footer style
        EditorGUI.indentLevel--;
        //GUI.backgroundColor = new Color(.9f, .5f, .9f);

        



        #endregion

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }



        serializedObject.ApplyModifiedProperties();
    }

}