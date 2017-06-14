using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBallEffect : MonoBehaviour
{

	[SerializeField] private Transform _targetTransform;
	[SerializeField] private Vector3 _offset;
	[SerializeField] private GameObject _voidBall;
	[SerializeField] private AudioClip _voidBallSound;
	[SerializeField] private AnimationCurve _scaleCurve;
	[SerializeField] private float _growTime;
	[SerializeField] private float _additiveNoiseFactor;
	[SerializeField] private float _blackoutTime;
	[SerializeField] private FireBallControl _fireBallScript;
	[SerializeField] private AstroidSummonControl _asteroidScript;
 

	private float _time;
	private GameObject _voidBallInstance;
	private Vector3 _currScale;

	// Use this for initialization
	void Start ()
	{
		Invoke ("Activate", 30f);
	}

	private void Update ()
	{
		_time += Time.deltaTime;
	}

	public void Activate ()
	{
		_time = 0f;
		_voidBallInstance = Instantiate (_voidBall, _targetTransform.position + _offset, Quaternion.identity, null);
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
		Destroy (_voidBallInstance);
		_targetTransform.gameObject.GetComponent<InverseColorEffect> ().ControlValue = 0f;

		_fireBallScript.IsFrozen = true;
		_asteroidScript.IsFrozen = true;
		//TODO: Invoke UI script
		Invoke ("EndEffect", _blackoutTime);
	}

	private void EndEffect ()
	{
		_targetTransform.gameObject.GetComponent<InverseColorEffect> ().ControlValue = 1f;
		_fireBallScript.IsFrozen = false;
		_asteroidScript.IsFrozen = false;
		print ("Void effect out");
	}

}
