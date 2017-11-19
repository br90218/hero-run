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
	[SerializeField] private float _aliveTime = 100f;
	[SerializeField] private ParticleSystem _childParticleSystem;
	[SerializeField] private float _maximumPower = 5f;
	[SerializeField] private float _perlinNoiseTraverseSpeed = 1f;
	[SerializeField] private float _shakeIntensity = 0.02f;
	[SerializeField] private bool _blockingEnable = true;
	public GameObject UI;
	private Material _swordMaterial;
	private SwordState _currState;
	private SwordState _nextState;
	private float _power;
	private int _noiseXCoordinate;
	private int _noiseYCoordinate;
	private Vector3 m_velocity;
	public GameObject m_target;
	public float m_speed, m_turn, m_angle, m_distance;
	int auto;
	// Use this for initialization
	void Start ()
	{
		_maximumPower = 2;
		m_velocity = new Vector3 (0, 0, 0);
		auto = 1;
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
			GetComponent<BoxCollider> ().enabled = true;
			if (m_velocity.magnitude == 0) {
				m_speed = (_power + 1) * 4;
				m_angle = 20 - _power * 3;
				m_turn = (_blockingEnable) ? 0.01f / 3f : (0.01f) * (1 + _power / 2);
				m_velocity = transform.forward * m_speed;
				m_distance = Vector3.Distance (transform.position, m_target.transform.position); 
			}

			//TODO: implement sword shooting methods

			float distance = Vector3.Distance (transform.position, m_target.transform.position);
			transform.position += m_velocity * Time.deltaTime * m_speed;
			m_velocity += (m_target.transform.position - transform.position) * m_turn * auto;
			if (Vector3.Dot (transform.forward, m_target.transform.position - transform.position) < Mathf.Cos (Mathf.Deg2Rad * m_angle)) {
				auto = 0;
			}

			m_velocity.Normalize ();
			transform.forward = m_velocity;
			m_velocity *= m_speed;

			//transform.position += transform.forward * _shootingSpeed * Time.deltaTime;
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

	public void SetPowerParam (float speed, float turnRate, float angle)
	{
		m_speed = speed;
		m_turn = turnRate;
		m_angle = angle;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.gameObject == m_target) {
			print ("HIT!");
			UI.GetComponent<TimerUI> ().timer = (UI.GetComponent<TimerUI> ().timer > 1) ? UI.GetComponent<TimerUI> ().timer - 1 : UI.GetComponent<TimerUI> ().timer;
			Destroy (gameObject);
		}
	}
}
