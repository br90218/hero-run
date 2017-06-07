using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItem : MonoBehaviour
{
    private Rigidbody rb;
    private float addForceThreshold = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 HandVelocity, Vector3 HandAngularVelocity)
    {
        if (HandVelocity.magnitude >= addForceThreshold)
        {
            rb.AddForce(HandVelocity * 30f);
        }
        if (HandAngularVelocity.magnitude <= 1500f && HandAngularVelocity.magnitude >= 500f)
        {
            rb.AddTorque(HandAngularVelocity * 0.1f);
        }
    }
}
