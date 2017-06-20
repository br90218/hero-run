using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToExitPrepareScene : MonoBehaviour {
    public Text PC_Ready_Text;
    public Text VR_Ready_Text;
    public GameObject L_Controller;
    public GameObject R_Controller;

    private SteamVR_TrackedObject _R_trackedObj;
    private SteamVR_TrackedObject _L_trackedObj;
    private bool isPCReady;
    private float _GripTimer;
    private bool _isVRReady;
	// Use this for initialization
	void Start () {
        isPCReady = false;
        _isVRReady = false;
        _GripTimer = 0f;
        StartCoroutine("PCTextFlash");
        StartCoroutine("VRTextFlash");
	}

    private void Awake()
    {
        _R_trackedObj = R_Controller.GetComponent<SteamVR_TrackedObject>();
        _L_trackedObj = L_Controller.GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        var R_device = SteamVR_Controller.Input((int)_R_trackedObj.index);
        var L_device = SteamVR_Controller.Input((int)_L_trackedObj.index);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isPCReady = true;
            PC_Ready_Text.text = "Your Journey Will Begin Shortly...";
            StopCoroutine("PCTextFlash");
            PC_Ready_Text.color = Color.white;
        }
        if (R_device.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && L_device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            _GripTimer += Time.deltaTime;
        }
        else
        {
            _GripTimer = 0f;
        }
        if (_GripTimer > 2f)
        {
            _isVRReady = true;
            VR_Ready_Text.text = "Prepare to Defend Heaven...";
            StopCoroutine("VRTextFlash");
            VR_Ready_Text.color = Color.white;
        }
        if(_isVRReady && isPCReady)
        {
            SceneManager.LoadScene("Level01_VR");
        }
    }

    private IEnumerator PCTextFlash()
    {
        while (true)
        {
            PC_Ready_Text.color = Color.Lerp(Color.white, Color.clear, Mathf.PingPong(Time.time, 1));
            yield return new WaitForFixedUpdate();
        }
        
    }
    private IEnumerator VRTextFlash()
    {
        while (true)
        {
            VR_Ready_Text.color = Color.Lerp(Color.white, Color.clear, Mathf.PingPong(Time.time, 1));
            yield return new WaitForFixedUpdate();
        }

    }
}
