using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

	[SerializeField] private RespawnController _respawnController;
	[SerializeField] private int _checkpointNumber;

	private void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("PCPlayer")) {
			_respawnController.SetCheckpointFlag (_checkpointNumber);
		}
	}
}
