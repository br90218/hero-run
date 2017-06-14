using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{

	[SerializeField] private GameObject _leftController;
	[SerializeField] private GameObject _rightController;


	private SteamVR_TrackedObject _leftTrackedObj;
	private SteamVR_TrackedObject _rightTrackedObj;

	private void Awake ()
	{
		_leftTrackedObj = _leftController.GetComponent<SteamVR_TrackedObject> ();
		_rightTrackedObj = _rightController.GetComponent<SteamVR_TrackedObject> ();
	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		var deviceL = SteamVR_Controller.Input ((int)_leftTrackedObj.index);
		var deviceR = SteamVR_Controller.Input ((int)_rightTrackedObj.index);

	}
}
