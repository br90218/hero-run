using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class FogVolumeData : MonoBehaviour
{
    [SerializeField]
    bool _ForceNoRenderer;
    public bool ForceNoRenderer
    {
        get { return _ForceNoRenderer; }
        set
        {
            if (_ForceNoRenderer != value)
            {
                _ForceNoRenderer = value;
                ToggleFogVolumeRenderers();
            }
        }
    }
    [SerializeField]
    Camera _GameCamera;

    public Camera GameCamera
    {
        get { return _GameCamera; }
        set
        {
            if (_GameCamera != value)
            {
                _GameCamera = value;
                RefreshCamera();

            }
        }

    }

    void RefreshCamera()
    {
        //refresh all fog folumes assigned camera
        //print("Refresh");

        FindFogVolumes();
        foreach (FogVolume _FogVolumes in SceneFogVolumes)
        {
            _FogVolumes.AssignCamera();

        }
        ToggleFogVolumeRenderers();
    }

    [SerializeField]
    List<Camera> FoundCameras;

    void OnEnable()
    {
        Initialize();
    }

    void Initialize()
    {

        if (FoundCameras == null)
            FoundCameras = new List<Camera>();
        FindCamera();
        RefreshCamera();
        if (FoundCameras.Count == 0)
            Debug.Log("Definetly, no camera available for Fog Volume");
        

    }

    [SerializeField]
    FogVolume[] SceneFogVolumes;

    void FindFogVolumes()
    {

        SceneFogVolumes = (FogVolume[])FindObjectsOfType(typeof(FogVolume));

    }

    void Update()
    {
#if UNITY_EDITOR
        if (GameCamera == null)
        {

            Debug.Log("No Camera available for Fog Volume. Trying to find another one");
            Initialize();
        }

#endif
    }
    void ToggleFogVolumeRenderers()
    {
        if (FoundCameras != null)
            for (int i = 0; i < FoundCameras.Count; i++)
            {
                if (FoundCameras[i] != _GameCamera)
                {
                    if (FoundCameras[i].GetComponent<FogVolumeRenderer>())
                        FoundCameras[i].GetComponent<FogVolumeRenderer>().enabled = false;
                }
                else
                    if (FoundCameras[i].GetComponent<FogVolumeRenderer>() && !_ForceNoRenderer)
                    FoundCameras[i].GetComponent<FogVolumeRenderer>().enabled = true;
                else
                    FoundCameras[i].GetComponent<FogVolumeRenderer>().enabled = false;

            }
    }
    public void FindCamera()
    {
        //We will try to assign the typical MainCamera first. This search will be performed only when the field is null
        //This is just an initial attempt on assigning any camera available when the field 'Camera' is null.
        //We will be able to select any other camera later
        if (FoundCameras != null && FoundCameras.Count > 0) FoundCameras.Clear();
        //Find all cameras in scene and store
        Camera[] CamerasFound = (Camera[])FindObjectsOfType(typeof(Camera));
        for (int i = 0; i < CamerasFound.Length; i++)
            if (
                !CamerasFound[i].name.Contains("FogVolumeCamera")
                &&
                !CamerasFound[i].name.Contains("Shadow Camera"))//not you!
                FoundCameras.Add(CamerasFound[i]);

        if (GameCamera == null)
            GameCamera = Camera.main;

        //No MainCamera? Try to find any!
        if (GameCamera == null)
        {
            foreach (Camera FoundCamera in FoundCameras)
            {

                // Many effects may use hidden cameras, so let's filter a little bit until we get something valid
                if (FoundCamera.isActiveAndEnabled)
                    if (FoundCamera.gameObject.activeInHierarchy)
                        if (FoundCamera.gameObject.hideFlags == HideFlags.None)
                        {

                            GameCamera = FoundCamera;
                            break;
                        }

            }
        }
        if (GameCamera != null)
        {
            // Debug.Log("Fog Volume has been assigned with camera: " + GameCamera);
            if (FindObjectOfType<FogVolumeCamera>())
                FindObjectOfType<FogVolumeCamera>().SceneCamera = GameCamera;

        }
        
    }
    public Camera GetFogVolumeCamera
    {
        get
        {
            return GameCamera;
        }

    }

    void OnDisable()
    {
        FoundCameras.Clear();
        SceneFogVolumes = null;
    }
}
