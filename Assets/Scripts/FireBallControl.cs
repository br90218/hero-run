using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallControl : MonoBehaviour {
    public GameObject Controller;
    public GameObject ActivatedGameObject;

    private SteamVR_TrackedObject _trackedObj;
    private bool _magicAvailable;
	
	void Start () {
        _magicAvailable = true;
        deactivateAllChilds();
    }

    private void Awake()
    {
        _trackedObj = Controller.GetComponent<SteamVR_TrackedObject>();   
    }

    void Update () {
        var device = SteamVR_Controller.Input((int)_trackedObj.index);
        if (_magicAvailable == true && device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            activeAllChilds();
            this.transform.parent = Controller.transform;
            this.transform.localPosition = new Vector3(0f, 0f, 0f);
            this.transform.localRotation = Quaternion.identity;
            _magicAvailable = false;
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
        {
            deactivateAllChilds();
            this.transform.parent = null;
            StartCoroutine(shootMagic());
        }
    }

    private void deactivateAllChilds()
    {
        for (int i = 0; i <= 1; ++i)
        {
            this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void activeAllChilds()
    {
        for (int i = 0; i <= 1; ++i)
        {
            this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    IEnumerator shootMagic()
    {
        ActivatedGameObject.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        ActivatedGameObject.SetActive(false);
        _magicAvailable = true;
    }
}
