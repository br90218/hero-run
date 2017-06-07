using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowForController : MonoBehaviour
{
    public SteamVR_TrackedObject Controller;
    public GameObject ControllerGameObj;
    public float speedMultiplier = 2f;
    private bool grabbing = false;
    private Vector3 prev_velocity;
    private Vector3 velocity;

    private Vector3 prev_rot;
    private Vector3 rot;

    private Collider grabbingCal;

    void Awake()
    {
        Controller = ControllerGameObj.GetComponent<SteamVR_TrackedObject>();
        grabbing = false;
    }

    void Start()
    {
        prev_rot = transform.rotation.eulerAngles;
    }


    void Update()
    {
        var device = SteamVR_Controller.Input((int)Controller.index);
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Quaternion curHandRot = transform.rotation;
        }

        velocity = (transform.position - prev_velocity) / Time.deltaTime;
        prev_velocity = transform.position;

        rot = transform.rotation.eulerAngles - prev_rot;
        prev_rot = transform.rotation.eulerAngles;

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            //Debug.Log(grabbingCal.gameObject.GetComponent<Rigidbody>().velocity);
            if (grabbingCal != null)
            {
                grabbingCal.gameObject.transform.parent = null;
                grabbingCal.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbingCal.gameObject.GetComponent<Rigidbody>().velocity = grabbingCal.gameObject.GetComponent<ThrowForObject>().velocity * speedMultiplier;
                grabbingCal.gameObject.GetComponent<Rigidbody>().angularVelocity = grabbingCal.gameObject.GetComponent<ThrowForObject>().angularVelocity;
            }
            grabbing = false;
            grabbingCal = null;
        }
    }


    private void OnTriggerStay(Collider other)
    {

        var device = SteamVR_Controller.Input((int)Controller.index);
        if (!grabbing && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (other.gameObject.tag == "pickable")
            {
                other.gameObject.transform.SetParent(gameObject.transform);
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            grabbing = true;
            grabbingCal = other;
        }
    }
}
