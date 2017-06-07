using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingControl : MonoBehaviour {
    public GameObject Target;

    private float ForceMultiplier = 100;
    private WaitForSeconds wait = new WaitForSeconds(20f);
    private WaitForSeconds beforeTracking = new WaitForSeconds(3f);
    private bool isTracking = false;
    private Rigidbody rb;
    private Vector3 ForceDirection;

	// Use this for initialization
	void Start () {
        StartCoroutine(SelfDestruct());
        StartCoroutine(TimeBeforeTracking());
        rb = gameObject.GetComponent<Rigidbody>();
        ForceMultiplier = 100f;

        rb.velocity = new Vector3(0f, 10f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		if(isTracking)
        {
            ForceDirection = (Target.transform.position - gameObject.transform.position).normalized;
            rb.AddForce(ForceMultiplier * ForceDirection);
            if(rb.velocity.magnitude >= 10f)
            {
                transform.Translate(Vector3.forward * 10f * Time.deltaTime);
            }

        }
	}

    private IEnumerator TimeBeforeTracking()
    {
        yield return beforeTracking;
        isTracking = true;
    }

    private IEnumerator SelfDestruct()
    {
        yield return wait;
        Destroy(this.gameObject);
    }
}
