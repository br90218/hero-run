//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Valve.VR;

//public static class VRMatrix
//{


//    static Matrix4x4 HMDMatrix4x4ToMatrix4x4(HmdMatrix44_t input)
//    {
//        var m = Matrix4x4.identity;
//        m[0, 0] = input.m0; m[0, 1] = input.m1; m[0, 2] = input.m2; m[0, 3] = input.m3; m[1, 0] = input.m4; m[1, 1] = input.m5; m[1, 2] = input.m6; m[1, 3] = input.m7; m[2, 0] = input.m8; m[2, 1] = input.m9; m[2, 2] = input.m10; m[2, 3] = input.m11; m[3, 0] = input.m12; m[3, 1] = input.m13; m[3, 2] = input.m14; m[3, 3] = input.m15;
//        return m;
//    }
//    public static Matrix4x4 GetSteamVRProjectionMatrix(Camera cam, EVREye eye)

//    {
//        if (Application.isPlaying)
//            return HMDMatrix4x4ToMatrix4x4(SteamVR.instance.hmd.GetProjectionMatrix(eye, cam.nearClipPlane, cam.farClipPlane));
//        else
//            return Matrix4x4.identity;
//    }
//}
