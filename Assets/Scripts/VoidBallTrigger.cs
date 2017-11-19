using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBallTrigger : MonoBehaviour
{
	[SerializeField] private VoidBallEffect _effectController;
	[SerializeField] private float _regenerateTime;

	private void OnTriggerEnter ()
	{
		if (!_effectController.IsActivated ()) {
			_effectController.Activate ();
			StartCoroutine ("Vanish");
		}
	}

	private IEnumerator Vanish ()
	{
		GetComponent<MeshRenderer> ().enabled = false;
		GetComponent<SphereCollider> ().enabled = false;
		yield return new WaitForSeconds (_regenerateTime);
		GetComponent<MeshRenderer> ().enabled = true;
		GetComponent<SphereCollider> ().enabled = true;
	}
}
