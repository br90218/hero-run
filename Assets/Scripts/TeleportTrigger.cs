﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{

	[SerializeField] private GameObject _leftController;
	[SerializeField] private GameObject _rightController;


	private SteamVR_TrackedObject _leftTrackedObj;
	private SteamVR_TrackedObject _rightTrackedObj;
	private bool _isSearchingTeleportDest;

	private void Awake ()
	{
		_leftTrackedObj = _leftController.GetComponent<SteamVR_TrackedObject> ();
		_rightTrackedObj = _rightController.GetComponent<SteamVR_TrackedObject> ();
	}

	// Use this for initialization
	void Start ()
	{
		StartCoroutine ("SearchDestination");
	}
	
	// Update is called once per frame
	void Update ()
	{
		var deviceL = SteamVR_Controller.Input ((int)_leftTrackedObj.index);
		var deviceR = SteamVR_Controller.Input ((int)_rightTrackedObj.index);

		if (deviceL.GetTouch (SteamVR_Controller.ButtonMask.Trigger) && deviceR.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) {
			_isSearchingTeleportDest = true;
		} else {
			_isSearchingTeleportDest = false;
		}
	}

	private IEnumerator SearchDestination ()
	{
		GameObject marker = null;
		while (true) {
			if (_isSearchingTeleportDest) {
				RaycastHit hit;
				var ray = Physics.Raycast (transform.position, transform.forward, out hit);
				Debug.DrawRay (transform.position, transform.forward * 5000, Color.blue);
				if (hit.collider != null && hit.collider.CompareTag ("TeleportLocation")) {
					marker = hit.collider.gameObject;
					marker.GetComponent<TeleportMarkerTrigger> ().Activate (true);
				} else {
					if (marker != null) {
						marker.GetComponent<TeleportMarkerTrigger> ().Activate (false);
						marker = null;
					}
				}
			} else {
				if (marker != null) {
					marker.GetComponent<TeleportMarkerTrigger> ().Activate (false);
					marker = null;
				}
			}
			yield return new WaitForFixedUpdate ();
		}
	}


}
