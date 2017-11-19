using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBallEffect : MonoBehaviour
{

	[SerializeField] private Transform _targetCameraTransform;
	[SerializeField] private Vector3 _offset;
	[SerializeField] private GameObject _voidBall;
	[SerializeField] private AudioClip _voidBallSound;
	[SerializeField] private AnimationCurve _scaleCurve;
	[SerializeField] private float _growTime;
	[SerializeField] private float _additiveNoiseFactor;
	[SerializeField] private float _blackoutTime;
	[SerializeField] private FireBallControl _fireBallScript;
	[SerializeField] private AstroidSummonControl _asteroidScript;
	[SerializeField] private TeleportTrigger _teleportControl;
	[SerializeField] private float _shrinkTime;
	[SerializeField] private AnimationCurve _shrinkCurve;
 

	private float _time;
	private GameObject _voidBallInstance;
	private Vector3 _currScale;
	private bool _isActivated;

	// Use this for initialization
	void Start ()
	{
	}

	private void Update ()
	{
		_time += Time.deltaTime;
	}

	public void Activate ()
	{
		_isActivated = true;
		_time = 0f;
		_voidBallInstance = Instantiate (_voidBall, _targetCameraTransform.position + _offset, Quaternion.identity, null);
		GetComponent<AudioSource> ().clip = _voidBallSound;
		GetComponent<AudioSource> ().Play ();
		_voidBallInstance.transform.localScale = _currScale;
		StartCoroutine ("BallGrow");
		Invoke ("Blackout", _growTime);
	}

	private IEnumerator BallGrow ()
	{
		while (_time < _growTime) {
			var evaluated = _scaleCurve.Evaluate (_time);
			_currScale = new Vector3 (evaluated, evaluated, evaluated);
			var noise = Random.insideUnitSphere * _additiveNoiseFactor;
			_voidBallInstance.transform.localScale = _currScale + noise;
			yield return new WaitForFixedUpdate ();
		}
	}

	private void Blackout ()
	{
		//Destroy (_voidBallInstance);
		StartCoroutine ("BallJitter");
		_targetCameraTransform.gameObject.GetComponent<Camera> ().cullingMask = ~(1 << LayerMask.NameToLayer ("VoidBall"));
		_targetCameraTransform.gameObject.GetComponent<InverseColorEffect> ().ControlValue = 0f;
		_fireBallScript.IsFrozen = true;
		_asteroidScript.IsFrozen = true;
		_teleportControl.enabled = false;
		//TODO: Invoke UI script
		Invoke ("EndEffect", _blackoutTime);
	}

	private IEnumerator BallJitter ()
	{
		while (true) {
			var noise = Random.insideUnitSphere * _additiveNoiseFactor;
			_voidBallInstance.transform.localScale = _currScale + noise;
			yield return new WaitForFixedUpdate ();
		}

	}

	private void EndEffect ()
	{
		StopCoroutine ("BallJitter");
		StartCoroutine ("BallVanish");
		_targetCameraTransform.gameObject.GetComponent<Camera> ().cullingMask |= (1 << LayerMask.NameToLayer ("VoidBall"));
		_targetCameraTransform.gameObject.GetComponent<InverseColorEffect> ().ControlValue = 1f;
		_fireBallScript.IsFrozen = false;
		_asteroidScript.IsFrozen = false;
		_teleportControl.enabled = true;
		print ("Void effect out");

	}

	private IEnumerator BallVanish ()
	{
		var vanishTime = 0f;
		while (vanishTime < _shrinkTime) {
			var evaluated = _shrinkCurve.Evaluate (vanishTime);
			_currScale *= evaluated;
			var noise = Random.insideUnitSphere * _additiveNoiseFactor;
			_voidBallInstance.transform.localScale = _currScale + noise;
			vanishTime += Time.deltaTime;
			yield return new WaitForFixedUpdate ();
		}
		_isActivated = false;
		Destroy (_voidBallInstance);
	}

	public bool IsActivated ()
	{
		return _isActivated;
	}
}
