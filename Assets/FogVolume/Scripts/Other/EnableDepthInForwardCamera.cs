using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class EnableDepthInForwardCamera : MonoBehaviour {

	// Use this for initialization
	void OnEnable() {
       
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }
	
	// Update is called once per frame
	//void Update () {
        
 //       if (GetComponent<Camera>().depthTextureMode != DepthTextureMode.Depth)
 //           GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
 //   }
}
