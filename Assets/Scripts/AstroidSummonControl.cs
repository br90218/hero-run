using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidSummonControl : MonoBehaviour
{
	public GameObject Controller;
	public float coolTime = 3.5f;

	public bool IsFrozen;
	private SteamVR_TrackedObject _trackedObj;
	private bool _magicAvailable;
	private SteamVR_LaserPointer _pointer;
	private bool _pointerInitialized = false;
	private bool _aiming = false;
	private Transform _magicSign;
	private bool _cooling;

	void Start ()
	{
		_magicAvailable = true;
		deactivateAllChilds ();
		_magicSign = this.transform.FindChild ("magicSign");
	}

	private void Awake ()
	{
		_trackedObj = Controller.GetComponent<SteamVR_TrackedObject> ();
		_pointer = Controller.GetComponent<SteamVR_LaserPointer> ();
		if (_pointer == null && Controller.GetComponent<SteamVR_LaserPointer> () == null)
			_pointer = Controller.AddComponent<SteamVR_LaserPointer> () as SteamVR_LaserPointer;
	}


	void Update ()
	{
		if (!_pointerInitialized && Controller.transform.FindChild ("New Game Object") != null) {
			Controller.transform.FindChild ("New Game Object").gameObject.SetActive (false);
			_pointerInitialized = true;
		}
        
		var device = SteamVR_Controller.Input ((int)_trackedObj.index);

		if (IsFrozen) {
			return;
		}

		if (_magicAvailable == true && device.GetTouchDown (SteamVR_Controller.ButtonMask.Trigger)) {
			_magicSign.gameObject.SetActive (true);
			Controller.transform.FindChild ("New Game Object").gameObject.SetActive (true);
			_magicAvailable = false;
			_aiming = true;
		}

		if (_aiming == true && device.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) {
			Ray ray = new Ray (Controller.transform.position, Controller.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (_magicSign.gameObject.activeSelf == false)
					_magicSign.gameObject.SetActive (true);
				_magicSign.position = hit.point;
				_magicSign.up = hit.normal;
			} else {
				_magicSign.gameObject.SetActive (false);
			}
		}

		if (_aiming == true && device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {
			_aiming = false;
			Controller.transform.FindChild ("New Game Object").gameObject.SetActive (false);
			Ray ray = new Ray (Controller.transform.position, Controller.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				StartCoroutine (shootMagic ());
			} else {
				_magicAvailable = true;
			}
		}

		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {               //To prevent stucking in a state
			_cooling = true;
			StartCoroutine (resetMagicAvailability ());
		}
	}

	IEnumerator shootMagic ()
	{
		Ray ray = new Ray (Controller.transform.position, Controller.transform.forward);
		RaycastHit hit;        
		if (Physics.Raycast (ray, out hit)) {
			this.transform.position = hit.point + new Vector3 (0f, 15f, 0f) + Random.insideUnitSphere * 7;

			this.transform.LookAt (hit.point);
			Vector3 rot = this.transform.rotation.eulerAngles;
			rot = new Vector3 (rot.x + 180, rot.y, rot.z);
			this.transform.rotation = Quaternion.Euler (rot);
			_magicSign.position = hit.point;
			_magicSign.up = hit.normal;
			activateAllChilds ();
			yield return new WaitForSeconds (coolTime);
			deactivateAllChilds ();
			_magicAvailable = true;
		}
		yield return null;
	}

	IEnumerator resetMagicAvailability ()
	{
		if (!_cooling) {
			_cooling = true;
			yield return new WaitForSeconds (coolTime + 0.5f);
			if (!_magicAvailable)
				_magicAvailable = true;
			if (!_aiming)
				_aiming = false;
			_cooling = false;
		}
		yield return null;
	}

	private void deactivateAllChilds ()
	{
		for (int i = 0; i <= 6; ++i) {
			this.gameObject.transform.GetChild (i).gameObject.SetActive (false);
		}
	}

	private void activateAllChilds ()
	{
		for (int i = 0; i <= 5; ++i) {
			this.gameObject.transform.GetChild (i).gameObject.SetActive (true);
		}
	}
}
