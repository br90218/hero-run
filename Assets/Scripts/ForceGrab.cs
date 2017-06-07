using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGrab : MonoBehaviour
{
    private SteamVR_Controller.Device controller;
    private LineRenderer lRender;
    private Vector3[] positions;//For displayed line's start and end positions
    private GrabItem grabbable; //reference to the object's GrabItem script
    private bool grabbed;
    private Vector3 oldpos;      //For hand position calculation
    private Vector3 newpos;
    private Vector3 delta_pos;
    public Vector3 velocity;
    Quaternion lastRotation;     //Holds the previous frames rotation
    float magnitude;
    Vector3 axis;                //References to the relevent axis angle variables
    Vector3 angularVelocity;

    void Start()
    {
        SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        lRender = GetComponent<LineRenderer>();
        positions = new Vector3[2];
        oldpos = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        grab();
        calculateVelocity();
        calculateAngularVelocity();
    }

    private GrabItem RaycastForGrabbableObject()
    {
        RaycastHit hit;
        Ray r = new Ray(transform.position, transform.forward);

        Debug.DrawRay(transform.position, transform.forward);

        if (Physics.Raycast(r, out hit, Mathf.Infinity)
            && hit.collider.gameObject.GetComponent<GrabItem>() != null)
        {
            DisplayLine(true, hit.point);
            return hit.collider.gameObject.GetComponent<GrabItem>();
        }
        else
        {
            DisplayLine(false, transform.position);
            return null;
        }
    }

    private void grab()
    {
        if (!grabbed)
        {
            grabbable = RaycastForGrabbableObject();
            if (!grabbable) return;
        }

        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {// force grab
            grabbed = true;
            DisplayLine(false, transform.position);
        }
        else if (controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {// force move
            grabbable.Move(velocity, angularVelocity);
        }
        else if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {// release
            grabbed = false;
        }

    }

    void DisplayLine(bool display, Vector3 endpoint)
    {
        lRender.enabled = display;
        positions[0] = transform.position;
        positions[1] = endpoint;
        lRender.SetPositions(positions);
    }

    private void calculateVelocity()
    {
        newpos = transform.position;
        delta_pos = (newpos - oldpos);
        velocity = delta_pos / Time.deltaTime;
        oldpos = newpos;
        newpos = transform.position;
    }

    private void calculateAngularVelocity()
    {
        //The fancy, relevent math
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out magnitude, out axis);
        angularVelocity = (axis * magnitude) / Time.deltaTime;
        lastRotation = transform.rotation;
    }
}

