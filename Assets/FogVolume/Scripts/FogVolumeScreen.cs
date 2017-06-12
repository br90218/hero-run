using UnityEngine;
[ExecuteInEditMode]
public class FogVolumeScreen : MonoBehaviour
{
    [Header("Scene blur")]
    [SerializeField]
    [Range(.1f, 5)]
    float _Falloff = 1;
    int screenX;
    int screenY;
    Shader _BlurShader = null;
    Camera UniformFogCamera;
    GameObject UniformFogCameraGO;
    SPSRRT_helper _CameraRender;
    [HideInInspector]
    public Camera SceneCamera;
    RenderTexture RT_FogVolumeConvolution;
   
    [HideInInspector]
    public int FogVolumeLayer = -1;
    [SerializeField]
    [HideInInspector]
    string _FogVolumeLayerName = "FogVolumeUniform";
    public string FogVolumeLayerName
    {
        get { return _FogVolumeLayerName; }
        set
        {
            if (_FogVolumeLayerName != value)
                SetFogVolumeLayer(value);
        }
    }

    void SetFogVolumeLayer(string NewFogVolumeLayerName)
    {
        _FogVolumeLayerName = NewFogVolumeLayerName;
        FogVolumeLayer = LayerMask.NameToLayer(_FogVolumeLayerName);
    }
    void OnValidate()
    {
        SetFogVolumeLayer(_FogVolumeLayerName);
    }
    Material _BlurMaterial = null;
    Material BlurMaterial
    {
        get
        {
            if (_BlurMaterial == null)
            {
                _BlurMaterial = new Material(_BlurShader);
                _BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _BlurMaterial;
        }
    }
   
    [Range(0, 10)]
    public int iterations = 3;
    [Range(0, 1)]
    public float _Dither = .8f;
    [Range(0.0f, 1.0f)]
    public float blurSpread = 0.6f;

    //BLOOM stuff

    public enum BlurType
    {
        Standard = 0,
        Sgx = 1,
    }


    [Header("Bloom")]
    [Range(1, 5)]
    public int _BloomDowsample = 8;
    [Range(0.0f, 1.5f)]
    public float threshold = 0.35f;
    [Range(0.0f, 2.5f)]
    public float intensity = 2.5f;
    [Range(0, 1)]
    public float _Saturation=1;
    [Range(0, 5f)]
    public float blurSize = 1;


    [Range(1, 10)]
    public int blurIterations = 4;

    BlurType blurType = BlurType.Standard;

    Shader fastBloomShader = null;
    Material _fastBloomMaterial = null;
    Material fastBloomMaterial
    {
        get
        {
            if (_fastBloomMaterial == null)
            {
                _fastBloomMaterial = new Material(fastBloomShader);
                _fastBloomMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _fastBloomMaterial;
        }
    }

    void CreateUniformFogCamera()
    {
        UniformFogCameraGO = GameObject.Find("Uniform Fog Volume Camera");
        if (UniformFogCameraGO == null)
        {
            UniformFogCameraGO = new GameObject();
            UniformFogCameraGO.name = "Uniform Fog Volume Camera";
            if (UniformFogCamera == null)
                UniformFogCamera = UniformFogCameraGO.AddComponent<Camera>();

            UniformFogCamera.backgroundColor = new Color(0, 0, 0, 0);
            UniformFogCamera.clearFlags = CameraClearFlags.SolidColor;
            UniformFogCamera.renderingPath = RenderingPath.Forward;
            UniformFogCamera.enabled = false;
            UniformFogCamera.farClipPlane = SceneCamera.farClipPlane;
            _CameraRender = UniformFogCameraGO.AddComponent<SPSRRT_helper>();

#if UNITY_5_6_OR_NEWER
            UniformFogCamera.GetComponent<Camera>().allowMSAA = false;
#endif
        }
        else
        {
            UniformFogCamera = UniformFogCameraGO.GetComponent<Camera>();
            _CameraRender = UniformFogCameraGO.GetComponent<SPSRRT_helper>();

        }

        //UniformFogCamera.depthTextureMode = DepthTextureMode.Depth;
        // UniformFogCameraGO.hideFlags = HideFlags.None;
        UniformFogCameraGO.hideFlags = HideFlags.HideInHierarchy;

        if (_CameraRender == null)
            _CameraRender = UniformFogCameraGO.AddComponent<SPSRRT_helper>();

        _CameraRender.SceneCamera = SceneCamera;
        _CameraRender.SecondaryCamera = UniformFogCamera;
        initFOV = SceneCamera.fieldOfView;
    }
    float initFOV;

    void OnEnable()
    {
        SceneCamera = gameObject.GetComponent<Camera>();

        _BlurShader = Shader.Find("Hidden/FogVolumeDensityFilter");
        if (_BlurShader == null) print("Hidden/FogVolumeDensityFilter #SHADER ERROR#");

        fastBloomShader = Shader.Find("Hidden/FogVolumeBloom");
        if (fastBloomShader == null) print("Hidden/FogVolumeBloom #SHADER ERROR#");

        CreateUniformFogCamera();

    }

    protected void OnDisable()
    {
        if (_BlurMaterial)
        {
            DestroyImmediate(_BlurMaterial);
        }

        if (_fastBloomMaterial)
        {
            DestroyImmediate(_fastBloomMaterial);
        }
    }

  

    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 5 * blurSpread;
        Graphics.BlitMultiTap(source, dest, BlurMaterial,
                               new Vector2(-off, -off),
                               new Vector2(-off, off),
                               new Vector2(off, off),
                               new Vector2(off, -off)
            );
    }
    public RenderTextureFormat GetRTFormat()
    {

#if UNITY_5_6_OR_NEWER
        return (SceneCamera.allowHDR == true) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
#else
        return (SceneCamera.hdr == true) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGBHalf;
#endif

    }
    public void ReleaseRT(RenderTexture rt)
    {
        if (rt != null)
        {
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
        }
    }
    public RenderTextureReadWrite GetRTReadWrite()
    {
        //return RenderTextureReadWrite.Default;
#if UNITY_5_6_OR_NEWER
        return (SceneCamera.allowHDR) ? RenderTextureReadWrite.Default : RenderTextureReadWrite.Linear;
#else
        return (SceneCamera.hdr) ? RenderTextureReadWrite.Default : RenderTextureReadWrite.Linear;
#endif
    }
    protected void GetRT(ref RenderTexture rt, int2 size, string name)
    {

        // Release existing one
        ReleaseRT(rt);
        rt = RenderTexture.GetTemporary(size.x, size.y, 0, GetRTFormat(), GetRTReadWrite());
        rt.filterMode = FilterMode.Bilinear;
        rt.name = name;
        rt.wrapMode = TextureWrapMode.Clamp;

    }

    public void ConvolveFogVolume()
    {
        if (UniformFogCameraGO == null) CreateUniformFogCamera();

        int2 resolution = new int2(screenX, screenY);

        GetRT(ref RT_FogVolumeConvolution, resolution, "RT_FogVolumeConvolution");
        _CameraRender.targetTexture = RT_FogVolumeConvolution;
        _CameraRender.Render();
        Shader.SetGlobalTexture("RT_FogVolumeConvolution", RT_FogVolumeConvolution);
    }
    public bool SceneBloom = false;
    #region instance
    private static FogVolumeScreen _instance;
    public static FogVolumeScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FogVolumeScreen>();
                
            }

            return _instance;
        }
    }
    #endregion
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        #region Density convolution
        UniformFogCamera.cullingMask = 1 << FogVolumeScreen.instance.FogVolumeLayer; 
        Shader.SetGlobalFloat("FOV_compensation", initFOV / SceneCamera.fieldOfView);
        BlurMaterial.SetFloat("_Falloff", _Falloff);
        BlurMaterial.SetFloat("_Dither", _Dither);
        ConvolveFogVolume();
        screenX = source.width;
        screenY = source.height;
        RenderTexture RT_DensityBlur = RenderTexture.GetTemporary(screenX, screenY, 0, source.format);

        Graphics.Blit(source, RT_DensityBlur);

        for (int i = 0; i < iterations; i++)
        {
            RenderTexture RT_DensityBlur2 = RenderTexture.GetTemporary(screenX, screenY, 0, source.format);
            FourTapCone(RT_DensityBlur, RT_DensityBlur2, i);
            RenderTexture.ReleaseTemporary(RT_DensityBlur);
            RT_DensityBlur = RT_DensityBlur2;
        }
        //  Graphics.Blit(RT_DensityBlur, destination);       

        #endregion

        #region Bloom

        float widthMod = 2.0f / (float)_BloomDowsample;
        fastBloomMaterial.SetFloat("_Saturation", _Saturation);
        fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshold, intensity));
        var rtW = source.width / _BloomDowsample;
        var rtH = source.height / _BloomDowsample;

        // downsample
        RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
        rt.filterMode = FilterMode.Bilinear;

        if (!SceneBloom)
            Graphics.Blit(RT_FogVolumeConvolution, rt, fastBloomMaterial, 1);
        else
            Graphics.Blit(RT_DensityBlur, rt, fastBloomMaterial, 1);

        var passOffs = blurType == BlurType.Standard ? 0 : 2;

        for (int i = 1; i < blurIterations; i++)
        {
            fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + (i * 1.0f), 0.0f, threshold, intensity));

            // vertical blur
            RenderTexture rt2 = RenderTexture.GetTemporary(rtW / i, rtH / i, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, fastBloomMaterial, 2 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;

            // horizontal blur
            rt2 = RenderTexture.GetTemporary(rtW / i, rtH / i, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, fastBloomMaterial, 3 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        fastBloomMaterial.SetTexture("_Bloom", rt);

        Graphics.Blit(RT_DensityBlur, destination, fastBloomMaterial, 0);
        RenderTexture.ReleaseTemporary(RT_DensityBlur);
        RenderTexture.ReleaseTemporary(rt);
        #endregion

    }
}
