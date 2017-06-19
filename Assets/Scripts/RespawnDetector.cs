using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnDetector : MonoBehaviour
{

	private void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("PCPlayer")) {
			other.GetComponent<PCPlayerControl> ().Death ();
		}
		if (other.gameObject.layer == LayerMask.NameToLayer ("Magic")) {
			Destroy (other.gameObject);
		}
	}
}
