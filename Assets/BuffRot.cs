using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffRot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up*2+Vector3.right+Vector3.forward, 5);
	}
}
