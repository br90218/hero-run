using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MagicModeManager : MonoBehaviour {
    public GameObject rightController;
    public GameObject leftController;
    public SteamVR_TrackedObject rightControllerTrackedObj;
    public MagicMode currentMode;

    private GameObject[] pickables;
    private MagicMode previousMode;
    public enum MagicMode
    {
        TheForce,
        MagicBall,
        MagicBow,
        BigMagic
    };

    void Awake()
    {
        rightControllerTrackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () {
        currentMode = MagicMode.TheForce;
        setupMode();
	}
	
	// Update is called once per frame
	void Update () {
        var device = SteamVR_Controller.Input((int)rightControllerTrackedObj.index);
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (device.GetAxis().x > 0 && device.GetAxis().y > 0)
                currentMode = MagicMode.TheForce;
            if (device.GetAxis().x < 0 && device.GetAxis().y > 0)
                currentMode = MagicMode.MagicBall;
            if (device.GetAxis().x < 0 && device.GetAxis().y < 0)
                currentMode = MagicMode.MagicBow;
            if (device.GetAxis().x > 0 && device.GetAxis().y < 0)
                currentMode = MagicMode.BigMagic;
        }

        if (currentMode != previousMode)
        {
            setupMode();
        }
	}

    private void setupMode()
    {
        switch (currentMode)
        {
            case MagicMode.TheForce:
                switchToForceMode();
                previousMode = currentMode;
                break;
            case MagicMode.MagicBall:
                switchToMagicBallMode();
                previousMode = currentMode;
                break;
            case MagicMode.MagicBow:
                switchToMagicBowMode();
                previousMode = currentMode;
                break;
            case MagicMode.BigMagic:
                switchToBigMagicMode();
                previousMode = currentMode;
                break;

        }
    }

    private void disableAllModes()
    {
        pickables = GameObject.FindGameObjectsWithTag("pickable");
        for (int i = 0; i < pickables.Length; i++)
        {
            pickables[i].GetComponent<VRTK_InteractableObject>().isGrabbable = false;
        }

        rightController.GetComponent<LineRenderer>().enabled = false;
        rightController.GetComponent<ForceGrab>().enabled = false;

        foreach (Transform child in rightController.transform)
        {
            if (child.name == "MagicArrow")
            {
                child.gameObject.SetActive(false);
            }
        }

        leftController.GetComponent<LineRenderer>().enabled = false;
        leftController.GetComponent<BigMagic>().enabled = false;
        leftController.GetComponent<ForceGrab>().enabled = false;
        foreach (Transform child in leftController.transform)
        {
            if (child.name == "MagicBowl")
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void switchToForceMode()
    {
        disableAllModes();
        rightController.GetComponent<LineRenderer>().enabled = true;
        leftController.GetComponent<LineRenderer>().enabled = true;
        rightController.GetComponent<ForceGrab>().enabled = true;
        leftController.GetComponent<ForceGrab>().enabled = true;
    }

    private void switchToMagicBallMode()
    {
        disableAllModes();
        pickables = GameObject.FindGameObjectsWithTag("pickable");
        for (int i = 0; i < pickables.Length; i++)
        {
            pickables[i].GetComponent<VRTK_InteractableObject>().isGrabbable = true;
        }
    }

    private void switchToMagicBowMode()
    {
        disableAllModes();

        foreach (Transform child in rightController.transform)
        {
            if (child.name == "MagicArrow")
            {
                child.gameObject.SetActive(true);
            }
        }

        foreach (Transform child in leftController.transform)
        {
            if (child.name == "MagicBowl")
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    private void switchToBigMagicMode()
    {
        disableAllModes();
        leftController.GetComponent<BigMagic>().enabled = true;
    }


}
