using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShoot : MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    public ArrowDirection ArrowDir;
    public LineRenderer bowString;
    public GameObject stringAttachOne;
    public GameObject stringAttachTwo;
    public bool isShot = false;

    private Rigidbody rb;
    //public LineRenderer line;
    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        isShot = false;
	}
	
	// Update is called once per frame
	void Update () {

        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Destroy(ArrowDir);
            //line.SetActive(false);
            gameObject.transform.parent = null;
            rb.velocity = transform.forward*50f;
            transform.localScale *= 5;

            isShot = true;            
        }
    }
}
