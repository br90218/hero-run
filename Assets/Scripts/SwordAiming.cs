using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordAiming : MonoBehaviour
{

	private enum State
	{
		Aiming,
		Shooting,
		Idle}

	;


	[SerializeField] private Transform[] _shealthTransforms;
	[SerializeField] private Camera _camera;
	[SerializeField] private float _aimingFOV;
	[SerializeField] private float _regularFOV = 60f;
	[SerializeField] private float _fovLerpFactor = 0.2f;
	[SerializeField] private GameObject _swordPrefab;
	[SerializeField] private float _respawnTime;
	[SerializeField] private RawImage _crossHair;
	[SerializeField] private GameObject _VRPlayer;
	public float m_speed = 1;
	public float m_turn = 1;
	public float m_angle = 90;
	public GameObject UI;

	private GameObject[] _swordsInstances;
	private Vector3[] _originalShealthLocations;
	private Quaternion[] _originalShealthRotations;
	private int _chosenSlot = -1;
	private State _currState;
	private State _nextState;

	// Use this for initialization
	void Start ()
	{
		m_speed = 10;
		_swordsInstances = new GameObject[_shealthTransforms.Length];
		_originalShealthLocations = new Vector3[_shealthTransforms.Length];
		_originalShealthRotations = new Quaternion[_shealthTransforms.Length];
		for (var i = 0; i < _shealthTransforms.Length; i++) {
			_originalShealthLocations [i] = _shealthTransforms [i].localPosition;
			_originalShealthRotations [i] = _shealthTransforms [i].localRotation;
			_swordsInstances [i] = Instantiate (_swordPrefab, _shealthTransforms [i].position, _shealthTransforms [i].rotation);
			_swordsInstances [i].GetComponent<Sword> ().FollowTransform = _shealthTransforms [i];
		}
		_currState = State.Idle;
		_nextState = State.Idle;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButton (1)) {
			_nextState = State.Aiming;
			if (Input.GetMouseButton (0)) {
				_nextState = State.Shooting;
			}
		} else {
			_nextState = State.Idle;
		}


		ApplyStateRules ();

		_currState = _nextState;
		_nextState = _currState;
	}

	private void ApplyStateRules ()
	{
		if (_currState == State.Idle) {
			for (var i = 0; i < _shealthTransforms.Length; i++) {
				_shealthTransforms [i].localPosition = _originalShealthLocations [i];
				_shealthTransforms [i].localRotation = _originalShealthRotations [i];
			}
			_camera.fieldOfView = Mathf.Lerp (_camera.fieldOfView, _regularFOV, _fovLerpFactor);
			_crossHair.enabled = false;
		} else if (_currState == State.Aiming) {
			for (var i = 0; i < _shealthTransforms.Length; i++) {
				_shealthTransforms [i].localPosition = _originalShealthLocations [i] + new Vector3 (0f, 0.9f, 0f);
				_shealthTransforms [i].rotation = _camera.transform.rotation;
			}
			_camera.fieldOfView = Mathf.Lerp (_camera.fieldOfView, _aimingFOV, _fovLerpFactor);
			_crossHair.enabled = true;

		} else if (_currState == State.Shooting) {
			for (var i = 0; i < _shealthTransforms.Length; i++) {
				_shealthTransforms [i].localPosition = _originalShealthLocations [i] + new Vector3 (0f, 0.9f, 0f);
				_shealthTransforms [i].rotation = _camera.transform.rotation;
			}
			if (_chosenSlot != -1) {
				if (_nextState != State.Shooting) {
					//Set Sword param
					_swordsInstances [_chosenSlot].GetComponent<Sword> ().SetPowerParam (m_speed, m_turn, m_angle);
					_swordsInstances [_chosenSlot].GetComponent<Sword> ().m_target = _VRPlayer;
					_swordsInstances [_chosenSlot].GetComponent<Sword> ().Shoot ();
					_swordsInstances [_chosenSlot].GetComponent<Sword> ().UI = UI;
					_swordsInstances [_chosenSlot] = null;
					StartCoroutine (RespawnSword (_chosenSlot));
					_chosenSlot = -1;
				} else {
					_swordsInstances [_chosenSlot].GetComponent<Sword> ().Charge ();
				}
			}
			for (var i = 0; i < _swordsInstances.Length; i++) {
				if (_swordsInstances [i] != null && _chosenSlot == -1) {
					_chosenSlot = i;
					break;
				}
			}
		}
	}

	private IEnumerator RespawnSword (int slot)
	{
		yield return new WaitForSeconds (_respawnTime);
		_swordsInstances [slot] = Instantiate (_swordPrefab, _shealthTransforms [slot].position, _shealthTransforms [slot].rotation);
		_swordsInstances [slot].GetComponent<Sword> ().FollowTransform = _shealthTransforms [slot];
	}
}
