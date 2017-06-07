using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowForObject : MonoBehaviour
{

    private Vector3 oldpos;
    private Vector3 newpos;
    private Vector3 delta_pos;
    public Vector3 velocity;

    //Holds the previous frames rotation
    Quaternion lastRotation;

    //References to the relevent axis angle variables
    float magnitude;
    Vector3 axis;


    public Vector3 angularVelocity
    {

        get
        {
            return (axis * magnitude) / Time.deltaTime;
        }

    }

    void Start()
    {
        oldpos = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        newpos = transform.position;
        delta_pos = (newpos - oldpos);
        velocity = delta_pos / Time.deltaTime;
        oldpos = newpos;
        newpos = transform.position;

        //The fancy, relevent math
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out magnitude, out axis);

        lastRotation = transform.rotation;
    }
}