using UnityEngine;
using System.Collections.Generic;

public class SPSRRT_helper : MonoBehaviour
{

    public Camera SceneCamera;
    public Camera SecondaryCamera;
    public RenderTexture targetTexture;
    public Shader CameraShader;
    private static bool s_InsideRendering = false;


    //private static readonly Rect LeftEyeRect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
  //  private static readonly Rect RightEyeRect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
    private static readonly Rect DefaultRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);


    public void Render()
    {

        if (s_InsideRendering)
        {
            return;
        }
        s_InsideRendering = true;


        //if (SceneCamera.stereoEnabled)
        //{
        //    if (SceneCamera.stereoTargetEye == StereoTargetEyeMask.Both || SceneCamera.stereoTargetEye == StereoTargetEyeMask.Left)
        //    {
        //        Vector3 eyePos = SceneCamera.transform.TransformPoint(SteamVR.instance.eyes[0].pos);
        //        Quaternion eyeRot = SceneCamera.transform.rotation * SteamVR.instance.eyes[0].rot;
        //        Matrix4x4 projectionMatrix = VRMatrix.GetSteamVRProjectionMatrix(SceneCamera, Valve.VR.EVREye.Eye_Left);

        //        RenderEye(targetTexture, eyePos, eyeRot, projectionMatrix, LeftEyeRect);
        //    }

        //    if (SceneCamera.stereoTargetEye == StereoTargetEyeMask.Both || SceneCamera.stereoTargetEye == StereoTargetEyeMask.Right)
        //    {
        //        Vector3 eyePos = SceneCamera.transform.TransformPoint(SteamVR.instance.eyes[1].pos);
        //        Quaternion eyeRot = SceneCamera.transform.rotation * SteamVR.instance.eyes[1].rot;
        //        Matrix4x4 projectionMatrix = VRMatrix.GetSteamVRProjectionMatrix(SceneCamera, Valve.VR.EVREye.Eye_Right);

        //        RenderEye(targetTexture, eyePos, eyeRot, projectionMatrix, RightEyeRect);
        //    }
        //}
        //else
        //{
            RenderEye(targetTexture, SceneCamera.transform.position, SceneCamera.transform.rotation, SceneCamera.projectionMatrix, DefaultRect);
       // }

        s_InsideRendering = false;
    }

    void RenderEye(RenderTexture targetTexture, Vector3 camPosition, Quaternion camRotation, Matrix4x4 camProjectionMatrix, Rect camViewport)
    {
        SecondaryCamera.ResetWorldToCameraMatrix();
        SecondaryCamera.transform.position = camPosition;
        SecondaryCamera.transform.rotation = camRotation;
        SecondaryCamera.projectionMatrix = camProjectionMatrix;
        SecondaryCamera.targetTexture = targetTexture;
        SecondaryCamera.rect = camViewport;

        if (CameraShader)
            SecondaryCamera.RenderWithShader(CameraShader, "RenderType");
        else

            SecondaryCamera.Render();//Getting the warning: OnRenderImage() possibly didn't write anything to the destination texture! 
        //fixed in 3.1.7

    }






}