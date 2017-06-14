using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMarkerTrigger : MonoBehaviour
{
	[SerializeField] private GameObject _targetMarker;

	private void Start ()
	{
		_targetMarker.SetActive (false);
	}

	public void Activate (bool value)
	{
		_targetMarker.SetActive (value);
	}
}
