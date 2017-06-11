
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteInEditMode]

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(SPSRRT_helper))]

public class FogVolumeCamera : MonoBehaviour
{
    [HideInInspector]
    public bool _GenerateDepth;
    [HideInInspector]
    public string FogVolumeResolution;
    //[HideInInspector]
    public RenderTexture RT_FogVolume;
    [HideInInspector]
    public int _Downsample;
    FogVolumeRenderer _FogVolumeRenderer;
    [HideInInspector]
    public Camera ThisCamera;
    [HideInInspector]
    public Camera SceneCamera;

    public int screenX
    {
        get
        {
            if (SceneCamera.stereoEnabled && !_FogVolumeRenderer.useRectangularStereoRT)
                return SceneCamera.pixelWidth / 2;
            else
                return SceneCamera.pixelWidth;
            
        }
    }

    public int screenY
    {
        get
        {
            return SceneCamera.pixelHeight;
         
        }
    }
    //  FogVolumeRenderer Combiner;
    public RenderTexture RT_Depth;
    Shader depthShader = null;
    [HideInInspector]
    public Material depthMaterial = null;
    public Material DepthMaterial
    {
        get
        {
            if (depthMaterial == null)
            {
                depthMaterial = new Material(depthShader);
                depthMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return depthMaterial;
        }
    }
    // float AspectRatio = 1;
    [HideInInspector]
    //[Range(0,1f)]
    public float upsampleDepthThreshold = .02f;

    #region BILATERAL_UPSAMPLING_DATA

    [System.Serializable]
    public enum UpsampleMode
    {
        DOWNSAMPLE_MIN,
        DOWNSAMPLE_MAX,
        DOWNSAMPLE_CHESSBOARD
    };

    // [SerializeField]
    UpsampleMode _upsampleMode;
    public UpsampleMode upsampleMode
    {

        set
        {
            if (value != _upsampleMode)
                SetUpsampleMode(value);
        }
        get
        {
            return _upsampleMode;
        }
    }
    void SetUpsampleMode(UpsampleMode value)
    {

        _upsampleMode = value;
        UpdateBilateralDownsampleModeSwitch();

    }

    public static bool BilateralUpsamplingEnabled()
    {
        return (SystemInfo.graphicsShaderLevel >= 40);
    }

    // Downsampled depthBuffer
    public RenderTexture[] lowProfileDepthRT;
    void ReleaseLowProfileDepthRT()
    {
        if (lowProfileDepthRT != null)
        {
            for (int i = 0; i < lowProfileDepthRT.Length; i++)
                RenderTexture.ReleaseTemporary(lowProfileDepthRT[i]);

            lowProfileDepthRT = null;
        }
    }

    //  [SerializeField]
    bool _useBilateralUpsampling = false;
    public bool useBilateralUpsampling
    {
        get { return _useBilateralUpsampling; }
        set
        {
            if (_useBilateralUpsampling != value)
                SetUseBilateralUpsampling(value);
        }
    }

    public enum UpsampleMaterialPass
    {
        DEPTH_DOWNSAMPLE = 0,
        BILATERAL_UPSAMPLE = 1
    };
    // [SerializeField]
    Material bilateralMaterial;
    void SetUseBilateralUpsampling(bool b)
    {

        _useBilateralUpsampling = b && BilateralUpsamplingEnabled();
        if (_useBilateralUpsampling)
        {
            if (bilateralMaterial == null)
            {
                bilateralMaterial = new Material(Shader.Find("Hidden/Upsample"));
                if (bilateralMaterial == null)
                    Debug.Log("#ERROR# Hidden/Upsample");

                // refresh keywords
                UpdateBilateralDownsampleModeSwitch();
                ShowBilateralEdge(_showBilateralEdge);
            }
        }
        else
        {
            // release resources
            bilateralMaterial = null;
        }
    }

    void UpdateBilateralDownsampleModeSwitch()
    {
        if (bilateralMaterial != null)
        {
            switch (_upsampleMode)
            {
                case UpsampleMode.DOWNSAMPLE_MIN:
                    bilateralMaterial.EnableKeyword("DOWNSAMPLE_DEPTH_MODE_MIN");
                    bilateralMaterial.DisableKeyword("DOWNSAMPLE_DEPTH_MODE_MAX");
                    bilateralMaterial.DisableKeyword("DOWNSAMPLE_DEPTH_MODE_CHESSBOARD");
                    break;
                case UpsampleMode.DOWNSAMPLE_MAX:
                    bilateralMaterial.DisableKeyword("DOWNSAMPLE_DEPTH_MODE_MIN");
                    bilateralMaterial.EnableKeyword("DOWNSAMPLE_DEPTH_MODE_MAX");
                    bilateralMaterial.DisableKeyword("DOWNSAMPLE_DEPTH_MODE_CHESSBOARD");
                    break;
                case UpsampleMode.DOWNSAMPLE_CHESSBOARD:
                    bilateralMaterial.DisableKeyword("DOWNSAMPLE_DEPTH_MODE_MIN");
                    bilateralMaterial.DisableKeyword("DOWNSAMPLE_DEPTH_MODE_MAX");
                    bilateralMaterial.EnableKeyword("DOWNSAMPLE_DEPTH_MODE_CHESSBOARD");
                    break;
                default:
                    break;
            }
        }
    }

    // Debug option: shows the bilateral upsamping edge
    //[SerializeField]
    bool _showBilateralEdge = false;


    public bool showBilateralEdge
    {
        set
        {
            if (value != _showBilateralEdge)
                ShowBilateralEdge(value);
        }
        get
        {
            return _showBilateralEdge;
        }
    }
    public void ShowBilateralEdge(bool b)
    {

        _showBilateralEdge = b;
        if (bilateralMaterial)
        {

            if (showBilateralEdge)
                bilateralMaterial.EnableKeyword("VISUALIZE_EDGE");
            else
                bilateralMaterial.DisableKeyword("VISUALIZE_EDGE");
        }
    }

    #endregion
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
    void ShaderLoad()
    {

        depthShader = Shader.Find("Hidden/Fog Volume/Depth");

        if (depthShader == null) print("Hidden/Fog Volume/Depth #SHADER ERROR#");
        //   depthShader = Shader.Find("Hidden/Internal-DepthNormalsTexture");

        //  if (depthShader == null) print("Hidden/Internal-DepthNormalsTexture #SHADER ERROR#");

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
    public RenderTextureFormat GetRTFormat()
    {
        //   return RenderTextureFormat.DefaultHDR;
        if (!_FogVolumeRenderer.TAA)
        {
#if UNITY_5_6_OR_NEWER
        return (SceneCamera.allowHDR == true) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
#else
            return (SceneCamera.hdr == true) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGBHalf;
#endif
        }
        else
            return RenderTextureFormat.ARGBHalf;
    }
    protected void GetRT(ref RenderTexture rt, int2 size, string name)
    {

        // Release existing one
        ReleaseRT(rt);
        rt = RenderTexture.GetTemporary(size.x, size.y, 0, GetRTFormat(), GetRTReadWrite());
        rt.filterMode = FilterMode.Bilinear;
        rt.name = name;
        rt.wrapMode = TextureWrapMode.Repeat;

    }
    public void ReleaseRT(RenderTexture rt)
    {
        if (rt != null)
        {
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
        }
    }

    SPSRRT_helper _CameraRender;
    RenderTextureFormat rt_DepthFormat;
    FogVolumeData _FogVolumeData;
    GameObject _FogVolumeDataGO;
    Texture2D nullTex;
    void OnEnable()
    {
        nullTex = MakeTex(Color.black);
        if (SystemInfo.SupportsRenderTextureFormat(rt_DepthFormat))
            rt_DepthFormat = RenderTextureFormat.RHalf;//not of the liking of small machines
        else
            rt_DepthFormat = RenderTextureFormat.Default;

        // SceneCamera = Camera.main;
        _FogVolumeDataGO = GameObject.Find("Fog Volume Data");
        if (_FogVolumeDataGO)
            _FogVolumeData = _FogVolumeDataGO.GetComponent<FogVolumeData>();


        if (_FogVolumeData)
            SceneCamera = _FogVolumeData.GetFogVolumeCamera;

        if (SceneCamera == null)
        {
            Debug.Log("FogVolumeCamera.cs can't get a valid camera from 'Fog Volume Data'\n Assigning Camera.main");
            SceneCamera = Camera.main;
        }

        ShaderLoad();
        SetUpsampleMode(_upsampleMode);
        ShowBilateralEdge(_showBilateralEdge);
        _FogVolumeRenderer = SceneCamera.GetComponent<FogVolumeRenderer>();

        if (SceneCamera.gameObject.GetComponent<FogVolumeRenderer>() == null)
            //    Combiner = SceneCamera.gameObject.GetComponent<FogVolumeRenderer>();
            //else
            SceneCamera.gameObject.AddComponent<FogVolumeRenderer>();

        ThisCamera = GetComponent<Camera>();
        //ThisCamera.depthTextureMode = DepthTextureMode.Depth;
        ThisCamera.enabled = false;

        ThisCamera.clearFlags = CameraClearFlags.SolidColor;
        ThisCamera.backgroundColor = new Color(0, 0, 0, 0);
        ThisCamera.farClipPlane = SceneCamera.farClipPlane;

        _CameraRender = GetComponent<SPSRRT_helper>();

        if (_CameraRender == null)
            _CameraRender = gameObject.AddComponent<SPSRRT_helper>();

        _CameraRender.SceneCamera = SceneCamera;
        _CameraRender.SecondaryCamera = ThisCamera;
    }


    void CameraUpdate()
    {

        //AspectRatio = (float)screenX / screenY;

        if (SceneCamera)
        {
            DepthMaterial.SetFloat("_AspectRatio", SceneCamera.aspect);


            ThisCamera.aspect = SceneCamera.aspect;
           // ThisCamera.nearClipPlane = .01f;
           // SceneCamera.nearClipPlane = .01f;
            //ThisCamera.farClipPlane = SceneCamera.farClipPlane;
            ThisCamera.nearClipPlane = SceneCamera.nearClipPlane;
#if UNITY_5_6_OR_NEWER
            ThisCamera.allowHDR = SceneCamera.allowHDR;
#else
            ThisCamera.hdr = SceneCamera.hdr;
#endif
            ThisCamera.fieldOfView = SceneCamera.fieldOfView;
        }
    }



    protected void Get_RT_Depth(ref RenderTexture rt, int2 size, string name)
    {
        // Release existing one
        ReleaseRT(rt);
        rt = RenderTexture.GetTemporary(size.x, size.y, 16, rt_DepthFormat, RenderTextureReadWrite.Linear);
        rt.filterMode = FilterMode.Bilinear;
        rt.name = name;
        rt.wrapMode = TextureWrapMode.Repeat;

    }

    public void RenderDepth()
    {
        if (_GenerateDepth)
        {
            Profiler.BeginSample("FogVolume Depth");
            //Gimme scene depth
            // ThisCamera.cullingMask = ~(1 << LayerMask.NameToLayer("FogVolume"));//hide FogVolume
            ThisCamera.cullingMask = SceneCamera.cullingMask;//Render the same than scene camera

            Get_RT_Depth(ref RT_Depth, new int2(screenX, screenY), "RT_Depth");

            _CameraRender.targetTexture = RT_Depth;
            _CameraRender.CameraShader = depthShader;
            _CameraRender.Render();

            Shader.SetGlobalTexture("RT_Depth", _CameraRender.targetTexture);
            _CameraRender.CameraShader = null;

            Profiler.EndSample();
        }
        else
            Shader.SetGlobalTexture("RT_Depth", nullTex);
    }


    public void Render()
    {
        CameraUpdate();
       
        if (_Downsample > 0)
        {

            RenderDepth();

            //Textured Fog
            ThisCamera.cullingMask = 1 << LayerMask.NameToLayer("FogVolume");//show Fog volume
            ThisCamera.cullingMask |= 1 << LayerMask.NameToLayer("FogVolumeShadowCaster");//show FogVolumeShadowCaster
            int2 resolution = new int2(screenX / _Downsample, screenY / _Downsample);
            FogVolumeResolution = resolution.x + " X " + resolution.y;
            GetRT(ref RT_FogVolume, resolution, "RT_FogVolume");
            _CameraRender.targetTexture = RT_FogVolume;
            _CameraRender.Render();
            // print("RT_FogVolume size: (" + RT_FogVolume.width + ", " + RT_FogVolume.height + ")");
            if (RT_FogVolume)
            {
                if (_FogVolumeRenderer.TAA && Application.isPlaying)
                    _FogVolumeRenderer._TAA.TAA(ref RT_FogVolume);
                Shader.SetGlobalTexture("RT_FogVolume", RT_FogVolume);
            }

            if (useBilateralUpsampling && _GenerateDepth)
            {
                #region BILATERAL_DEPTH_DOWNSAMPLE
                Profiler.BeginSample("FogVolume Upsample");
                // Compute downsampled depth-buffer for bilateral upsampling
                if (bilateralMaterial)
                {
                    ReleaseLowProfileDepthRT();
                    lowProfileDepthRT = new RenderTexture[_Downsample];

                    for (int downsampleStep = 0; downsampleStep < _Downsample; downsampleStep++)
                    {
                        int targetWidth = screenX / (downsampleStep + 1);
                        int targetHeight = screenY / (downsampleStep + 1);

                        int stepWidth = screenX / Mathf.Max(downsampleStep, 1);
                        int stepHeight = screenY / Mathf.Max(downsampleStep, 1);
                        Vector4 texelSize = new Vector4(1.0f / stepWidth, 1.0f / stepHeight, 0.0f, 0.0f);
                        bilateralMaterial.SetFloat("_UpsampleDepthThreshold", upsampleDepthThreshold);
                        bilateralMaterial.SetVector("_TexelSize", texelSize);
                        bilateralMaterial.SetTexture("_HiResDepthBuffer", RT_Depth);

                        lowProfileDepthRT[downsampleStep] =
                            RenderTexture.GetTemporary(targetWidth, targetHeight, 16, rt_DepthFormat, GetRTReadWrite());
                        lowProfileDepthRT[downsampleStep].name = "lowProfileDepthRT_" + downsampleStep;
                        Graphics.Blit(null, lowProfileDepthRT[downsampleStep], bilateralMaterial, (int)UpsampleMaterialPass.DEPTH_DOWNSAMPLE);
                    }
                    Shader.SetGlobalTexture("RT_Depth", lowProfileDepthRT[lowProfileDepthRT.Length - 1]);

                }


                #endregion

                #region BILATERAL_UPSAMPLE

                // Upsample convolution RT
                if (bilateralMaterial)
                {
                    for (int downsampleStep = _Downsample - 1; downsampleStep >= 0; downsampleStep--)
                    {
                        int targetWidth = screenX / Mathf.Max(downsampleStep, 1);
                        int targetHeight = screenY / Mathf.Max(downsampleStep, 1);
                       
                        // compute Low-res texel size
                        int stepWidth = screenX / (downsampleStep + 1);
                        int stepHeight = screenY / (downsampleStep + 1);
                        Vector4 texelSize = new Vector4(1.0f / stepWidth, 1.0f / stepHeight, 0.0f, 0.0f);
                        bilateralMaterial.SetVector("_TexelSize", texelSize);
                        bilateralMaterial.SetVector("_InvdUV", new Vector4(RT_FogVolume.width, RT_FogVolume.height, 0, 0));
                        // High-res depth texture

                        bilateralMaterial.SetTexture("_HiResDepthBuffer", RT_Depth);
                        bilateralMaterial.SetTexture("_LowResDepthBuffer", lowProfileDepthRT[downsampleStep]);

                        bilateralMaterial.SetTexture("_LowResColor", RT_FogVolume);
                        RenderTexture newRT = RenderTexture.GetTemporary(targetWidth, targetHeight, 0, GetRTFormat(), GetRTReadWrite());
                        newRT.filterMode = FilterMode.Bilinear;
                        Graphics.Blit(null, newRT, bilateralMaterial, (int)UpsampleMaterialPass.BILATERAL_UPSAMPLE);

                        // Swap and release
                        RenderTexture swapRT = RT_FogVolume;
                        RT_FogVolume = newRT;
                        RenderTexture.ReleaseTemporary(swapRT);
                    }
                }
                ReleaseLowProfileDepthRT();
                #endregion
                Profiler.EndSample();
            }

           

          

            Shader.SetGlobalTexture("RT_FogVolume", RT_FogVolume);
        }
        else
        {
            ReleaseRT(RT_FogVolume);
            FogVolumeResolution = Screen.width + " X " + Screen.height;
        }

    }



    void OnDisable()
    {
        if (ThisCamera)
            ThisCamera.targetTexture = null;
        DestroyImmediate(RT_FogVolume);
        ReleaseRT(RT_FogVolume);
        ReleaseRT(RT_Depth);
        DestroyImmediate(RT_Depth);
        DestroyImmediate(depthMaterial);
    }
    //GUIStyle labelStyle = new GUIStyle();
    //void OnGUI()
    //{
    //    labelStyle.normal.textColor = Color.white;
    //    labelStyle.fontSize = 50;
    //    GUI.Label(new Rect(10, 100, 100, 20), "RT_Depth format: "+RT_Depth.format.ToString(), labelStyle);
    //}
}
