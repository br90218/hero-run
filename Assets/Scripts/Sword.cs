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
	[SerializeField] private float _perlinNoiseTraverseSpeed = 1f;
	[SerializeField] private float _shakeIntensity = 0.02f;

	private Material _swordMaterial;
	private SwordState _currState;
	private SwordState _nextState;
	private float _power;
	private int _noiseXCoordinate;
	private int _noiseYCoordinate;
	// Use this for initialization
	void Start ()
	{
		_noiseXCoordinate = (int)(Random.value * 3f);
		_noiseYCoordinate = (int)(Random.value * 3f);
		_currState = SwordState.Follow;
		_nextState = SwordState.Follow;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		ApplyStateRules ();


		_currState = _nextState;
		_nextState = _currState;
	}

	private void ApplyStateRules ()
	{
		if (_currState == SwordState.Follow) {
			var noiseX = (Mathf.PerlinNoise (_noiseXCoordinate, Time.time * _perlinNoiseTraverseSpeed) * 2f - 1) * _shakeIntensity;
			var noiseY = (Mathf.PerlinNoise (_noiseYCoordinate, Time.time * _perlinNoiseTraverseSpeed) * 2f - 1) * _shakeIntensity;
			var noisePos = new Vector3 (noiseX, noiseY, 0);

			transform.position = Vector3.Lerp (transform.position, FollowTransform.position + noisePos, _followLerpFactor);
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
		transform.position += Random.insideUnitSphere * _power * 0.01f;
	}
		
}
