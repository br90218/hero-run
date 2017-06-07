using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookDirection : MonoBehaviour {
    public GameObject theOtherController;
    private Vector3 Direction;
	// Update is called once per frame
	void Update ()
    {
        Direction = transform.position - theOtherController.transform.position;
        transform.forward = Direction;
	}
}
