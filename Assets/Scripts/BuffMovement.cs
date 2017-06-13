using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMovement : MonoBehaviour {

	[SerializeField] private AnimationCurve _heightCurve;
	private float originalPosY;
	private float random;
	// Use this for initialization
	void Start () {
		originalPosY = transform.position.y;
		random = Random.value;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up*2+Vector3.right+Vector3.forward, 5);
		var newY = originalPosY + _heightCurve.Evaluate (Time.time + random);
		transform.position = new Vector3 (transform.position.x, newY, transform.position.z);
	}
}
