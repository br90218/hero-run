using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBallTrigger : MonoBehaviour
{
	[SerializeField] private VoidBallEffect _effectController;

	private void OnTriggerEnter ()
	{
		_effectController.Activate ();
		Destroy (gameObject);
	}
}
