using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallControl : MonoBehaviour
{
	public GameObject Controller;
	public GameObject ActivatedGameObject;
	public float FrameInterval = 0.01f;
	public bool IsFrozen;
	public float speed = 1;
	private SteamVR_TrackedObject _trackedObj;
	private bool _magicAvailable;
	private Vector3 posA, posB;
	private bool _fireBallAway;

	void Start ()
	{
		_magicAvailable = true;
		deactivateAllChilds ();

	}

	private void Awake ()
	{
		_trackedObj = Controller.GetComponent<SteamVR_TrackedObject> ();   
	}

	void Update ()
	{
		var device = SteamVR_Controller.Input ((int)_trackedObj.index);
		if (IsFrozen) {
			return;
		}
		if (_magicAvailable == true && device.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger)) {
			StartCoroutine ("TrackVelocity");
			activeAllChilds ();
			this.transform.parent = Controller.transform;
			this.transform.localPosition = new Vector3 (0f, 0f, 0f);
			this.transform.localRotation = Quaternion.identity;
			_magicAvailable = false;
		}
		if (!_fireBallAway && !_magicAvailable && device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {
			deactivateAllChilds ();
			this.transform.parent = null;
			_fireBallAway = true;
			StartCoroutine (shootMagic ());
		}
	}

	private void deactivateAllChilds ()
	{
		for (int i = 0; i <= 1; ++i) {
			this.gameObject.transform.GetChild (i).gameObject.SetActive (false);
		}
	}

	private void activeAllChilds ()
	{
		for (int i = 0; i <= 1; ++i) {
			this.gameObject.transform.GetChild (i).gameObject.SetActive (true);
		}
	}

	IEnumerator shootMagic ()
	{
		ActivatedGameObject.GetComponent<RFX4_TransformMotion> ().StartVector = (posB - posA) / FrameInterval * speed;
		ActivatedGameObject.SetActive (true);
		yield return new WaitForSeconds (3.5f);
		ActivatedGameObject.SetActive (false);
		_magicAvailable = true;
		_fireBallAway = false;
	}

	private IEnumerator TrackVelocity ()
	{
		var device = SteamVR_Controller.Input ((int)_trackedObj.index);
		while (device.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) {
			posA = posB;
			posB = _trackedObj.transform.position;
			yield return new WaitForSeconds (FrameInterval);
		}
	}
}
