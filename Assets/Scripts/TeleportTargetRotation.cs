using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTargetRotation : MonoBehaviour
{
	[SerializeField] private float _degreesPerSec = 360f;

	private float rotX;
	// Update is called once per frame
	void Update ()
	{
		rotX += Time.deltaTime * _degreesPerSec;
		transform.localRotation = Quaternion.Euler (new Vector3 (0, rotX, 0));
	}
}
