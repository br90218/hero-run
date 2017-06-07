using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCPlayerAttack : MonoBehaviour {
    public Transform VRPlayerHead;
    private Rigidbody rb;

    private Vector3 ShootDirection;
	// Use this for initialization
	void Start () {
        rb = this.gameObject.GetComponent<Rigidbody>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ShootDirection = (VRPlayerHead.position - this.transform.position).normalized;
            rb.velocity = ShootDirection * 30f;
        }
    }
}
