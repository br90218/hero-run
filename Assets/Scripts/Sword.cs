using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

	private enum SwordState
	{
		Follow,
		Shoot,
		Death}
	;

	public Transform FollowTransform;


	[SerializeField] private float _followLerpFactor = 0.2f;
	[SerializeField] private float _shootingSpeed = 1f;
	[SerializeField] private float _aliveTime = 5f;
	[SerializeField] private ParticleSystem _childParticleSystem;
	[SerializeField] private float _maximumPower = 5f;

	private Material _swordMaterial;
	private SwordState _currState;
	private SwordState _nextState;
	private float _power;
	// Use this for initialization
	void Start ()
	{
		_currState = SwordState.Follow;
		_nextState = SwordState.Follow;
	}
	
	// Update is called once per frame
	void Update ()
	{

		ApplyStateRules ();


		_currState = _nextState;
		_nextState = _currState;
	}

	private void ApplyStateRules ()
	{
		if (_currState == SwordState.Follow) {
			transform.position = Vector3.Lerp (transform.position, FollowTransform.position, _followLerpFactor);
			transform.rotation = Quaternion.Lerp (transform.rotation, FollowTransform.rotation, _followLerpFactor);
		} else if (_currState == SwordState.Shoot) {
			StartCoroutine ("KillCounter");
			transform.position += transform.forward * _shootingSpeed * Time.deltaTime;
		} else if (_currState == SwordState.Death) {
			//Death FX
			Destroy (gameObject);
		}
	}

	public void Shoot ()
	{
		_nextState = SwordState.Shoot;
	}

	private IEnumerator KillCounter ()
	{
		yield return new WaitForSeconds (_aliveTime);
		_nextState = SwordState.Death;
	}


	public void Charge ()
	{
		if (!_childParticleSystem.isPlaying) {
			_childParticleSystem.Play ();
		}
		_power = Mathf.Min (_power + Time.deltaTime, _maximumPower);
	}
		
}
