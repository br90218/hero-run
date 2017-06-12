using UnityEngine;

[ExecuteInEditMode]
public class FogVolumeRenderer : MonoBehaviour
{

    public string FogVolumeResolution;
    public bool RenderableInSceneView = true;

    public enum BlendMode
    {
        PremultipliedTransparency = (int)UnityEngine.Rendering.BlendMode.One,
        TraditionalTransparency = (int)UnityEngine.Rendering.BlendMode.SrcAlpha,

    };
    public BlendMode _BlendMode = BlendMode.TraditionalTransparency;
    public bool GenerateDepth = true;
    [SerializeField]
    [Range(0, 8)]
    public int _Downsample = 0;
    public bool useRectangularStereoRT = false;
    public bool BilateralUpsampling = false;
    public bool ShowBilateralEdge = false;
    [SerializeField]
    public FogVolumeCamera.UpsampleMode USMode = FogVolumeCamera.UpsampleMode.DOWNSAMPLE_MAX;
    public Camera ThisCamera = null;

    [HideInInspector]
    public FogVolumeCamera _FogVolumeCamera;
    // [SerializeField]
    GameObject _FogVolumeCameraGO;
    [SerializeField]
    [Range(0, .01f)]
    public float upsampleDepthThreshold = 7e-05f;
    public bool HDR;
    public bool TAA = false;
    public FogVolumeTAA _TAA = null;
    private FogVolumePlaydeadTAA.VelocityBuffer _TAAvelocity = null;

    void TAASetup()
    {
        if (_Downsample > 0 && TAA)
        {
            if (_FogVolumeCameraGO.GetComponent<FogVolumeTAA>() == null)
                _FogVolumeCameraGO.AddComponent<FogVolumeTAA>();
            _TAA = _FogVolumeCameraGO.GetComponent<FogVolumeTAA>();
            _TAAvelocity = _FogVolumeCameraGO.GetComponent<FogVolumePlaydeadTAA.VelocityBuffer>();
        }
    }

    void CreateFogCamera()
    {
        if (_Downsample > 0)
        {
            _FogVolumeCameraGO = new GameObject();
            _FogVolumeCameraGO.name = "FogVolumeCamera";
            _FogVolumeCamera = _FogVolumeCameraGO.AddComponent<FogVolumeCamera>();
            _FogVolumeCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            _FogVolumeCamera.GetComponent<Camera>().backgroundColor = new Color(0, 0, 0, 0);
            _FogVolumeCameraGO.hideFlags = HideFlags.HideInHierarchy;
            //_FogVolumeCameraGO.hideFlags = HideFlags.None;
            _FogVolumeCamera.GetComponent<Camera>().renderingPath = RenderingPath.Forward;
#if UNITY_5_6_OR_NEWER
            _FogVolumeCamera.GetComponent<Camera>().allowMSAA = false;
#endif
        }
    }

    void FindFogCamera()
    {
        _FogVolumeCameraGO = GameObject.Find("FogVolumeCamera");
        if (_FogVolumeCameraGO) DestroyImmediate(_FogVolumeCameraGO);//the RT is not created in VR on start. Resetting here for now

        CreateFogCamera();

        TAASetup();
    }
    Vector4 TexelSize = Vector4.zero;
    void TexelUpdate()
    {
        if (_FogVolumeCamera.RT_FogVolume)
        {
            TexelSize.x = 1.0f / _FogVolumeCamera.RT_FogVolume.width;
            TexelSize.y = 1.0f / _FogVolumeCamera.RT_FogVolume.height;
            TexelSize.z = _FogVolumeCamera.RT_FogVolume.width;
            TexelSize.w = _FogVolumeCamera.RT_FogVolume.height;
            Shader.SetGlobalVector("RT_FogVolume_TexelSize", TexelSize);
        }
        //  print(TexelSize);
    }

    void UpdateParams()
    {
        if (_FogVolumeCamera && _Downsample > 0)
        {
            _FogVolumeCamera.useBilateralUpsampling = BilateralUpsampling;
            _FogVolumeCamera._GenerateDepth = GenerateDepth;
            if (BilateralUpsampling && GenerateDepth)
            {
                _FogVolumeCamera.upsampleMode = USMode;

                _FogVolumeCamera.showBilateralEdge = ShowBilateralEdge;
                _FogVolumeCamera.upsampleDepthThreshold = upsampleDepthThreshold;
            }
            if (GenerateDepth)
                SurrogateMaterial.SetInt("_ztest", (int)UnityEngine.Rendering.CompareFunction.Always);
            else
                SurrogateMaterial.SetInt("_ztest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);

            if (!_TAA) TAASetup();

            if (_TAA && _TAA.enabled != TAA)
            {

                _TAA.enabled = TAA;
                _TAAvelocity.enabled = TAA;

            }

#if UNITY_5_6_OR_NEWER
        HDR= ThisCamera.allowHDR;
#else
            HDR = ThisCamera.hdr;
#endif
        }
    }
    Material SurrogateMaterial;
    void OnEnable()
    {
        ThisCamera = gameObject.GetComponent<Camera>();
        FindFogCamera();
        //ShaderLoad();
        SurrogateMaterial = (Material)Resources.Load("Fog Volume Surrogate");
        UpdateParams();
        //if(_FogVolumeCamera)
        //_Downsample = _FogVolumeCamera._Downsample;

    }
    public bool SceneBlur = true;
    
    void OnPreRender()
    {

#if UNITY_EDITOR
        UpdateParams();
        if (ThisCamera == null)
            ThisCamera = gameObject.GetComponent<Camera>();
#endif
        
            
        if (_Downsample > 0 && _FogVolumeCamera)

        {
           
                SurrogateMaterial.SetInt("_SrcBlend", (int)_BlendMode);
            Shader.EnableKeyword("_FOG_LOWRES_RENDERER");

            Shader.DisableKeyword("RENDER_SCENE_VIEW");
            // Profiler.BeginSample("FogVolume Render");
            _FogVolumeCamera.Render();
            // Profiler.EndSample();

            //  TexelUpdate();
            if (!RenderableInSceneView)
                Shader.EnableKeyword("RENDER_SCENE_VIEW");
            Shader.DisableKeyword("_FOG_LOWRES_RENDERER");

        }
        else
        {
            Shader.DisableKeyword("_FOG_LOWRES_RENDERER");
            Shader.DisableKeyword("RENDER_SCENE_VIEW");
        }

    }

    void Update()
    {
#if UNITY_EDITOR

        if (ThisCamera == null)
            ThisCamera = gameObject.GetComponent<Camera>();
#endif
        //#if UNITY_EDITOR
        //        // if destroyed...
        //        FindFogCamera();
        //#endif
        if (_FogVolumeCamera == null) FindFogCamera();

        if (_Downsample == 0) DestroyFogCamera();

        if (_Downsample > 0 && _FogVolumeCameraGO && this.isActiveAndEnabled)
        {

            ThisCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("FogVolume"));//hide FogVolume
            ThisCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("FogVolumeShadowCaster"));//hide FogVolumeShadowCaster
            FogVolumeResolution = _FogVolumeCamera.FogVolumeResolution;
            ThisCamera.cullingMask |= 1 << LayerMask.NameToLayer("FogVolumeSurrogate");//show
        }
        else
        {
            ThisCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("FogVolumeSurrogate"));//hide

            ThisCamera.cullingMask |= 1 << LayerMask.NameToLayer("FogVolume");//show FogVolume
            ThisCamera.cullingMask |= 1 << LayerMask.NameToLayer("FogVolumeShadowCaster");//show FogVolumeShadowCaster
            FogVolumeResolution = Screen.width + " X " + Screen.height;

        }

        if (_FogVolumeCameraGO)
            _FogVolumeCamera._Downsample = _Downsample;
        else
            CreateFogCamera();
    }
    void DestroyFogCamera()
    {
        if (_FogVolumeCameraGO &&
            _FogVolumeCameraGO.activeInHierarchy)//3.1.6
        {
            DestroyImmediate(_FogVolumeCameraGO);
        }

    }
    void OnDisable()
    {
        Shader.DisableKeyword("RENDER_SCENE_VIEW");
        Shader.DisableKeyword("_FOG_LOWRES_RENDERER");
        DestroyFogCamera();
        ThisCamera.cullingMask |= (1 << LayerMask.NameToLayer("FogVolume"));
        ThisCamera.cullingMask |= 1 << LayerMask.NameToLayer("FogVolumeShadowCaster");
    }
}
