using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBeam : MonoBehaviour
{

	[SerializeField] private GameObject _laserEffects;
	[SerializeField] private ParticleSystem _laserSparks;
	[SerializeField] private ParticleSystem _laserSmoke;
	[SerializeField] private AudioSource _laserChargeAudio;
	[SerializeField] private AudioSource _laserAudio;
	[SerializeField] private AudioSource _laserStopAudio;
	[SerializeField] private GameObject _laserChargeBeam;
	[SerializeField] private GameObject _smokeAndSparks;
	[SerializeField] private GameObject _scorchMark;
	[SerializeField] private float _shootingTime;
	public TeleportEffect _effectMaster;
	public GameObject fadingUI;
	public bool IsInPosition;
	bool blockCo = false;

	private ParticleSystem.EmissionModule _laserSparksEmitter;
	private ParticleSystem.EmissionModule _laserSmokeEmitter;
	private GameObject _scorchMarkClone;
	private bool _laserChargeFlag;
	private bool _isShooting;
	private float _timer;

	// Use this for initialization
	private void Start ()
	{
		_laserEffects.SetActive (false);
		_laserSparksEmitter = _laserSparks.emission;
		_laserSparksEmitter.enabled = false;

		_laserSmokeEmitter = _laserSmoke.emission;
		_laserSmokeEmitter.enabled = false;

		_laserChargeBeam.SetActive (false);
		_smokeAndSparks.SetActive (false);
		_smokeAndSparks.SetActive (true);
		_shootingTime = 1.5f;
		_scorchMarkClone = Instantiate (_scorchMark);

		_laserChargeAudio.Stop ();
		_laserAudio.Stop ();
		_laserStopAudio.Stop ();
		blockCo = false;

	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (IsInPosition) {
			if (_isShooting) {
				//return;
			} else {
				_laserChargeFlag = false;
				if (!_laserChargeAudio.isPlaying) {
					_laserChargeAudio.Play ();
				}
				_laserChargeBeam.SetActive (true);
				if (blockCo == false) {
					StartCoroutine ("LaserChargeWait");
					blockCo = true;
				}

			}

		} else {
			if (_isShooting) {
				//return;
			} else {
				_laserChargeFlag = true;
				_laserEffects.SetActive (false);
				_laserSparksEmitter.enabled = false;
				_laserSmokeEmitter.enabled = false;
				_laserAudio.Stop ();
				_laserStopAudio.Play ();
				_laserChargeBeam.SetActive (false);
			}
	
		}
		if (_isShooting) {
			if (_timer > _shootingTime) {
				
				_laserChargeFlag = true;
				_laserEffects.SetActive (false);
				_laserSparksEmitter.enabled = false;
				_laserSmokeEmitter.enabled = false;
				_laserAudio.Stop ();
				_laserStopAudio.Play ();
				_laserChargeBeam.SetActive (false);
				_isShooting = false;
				print ("Transform should be completed");
				blockCo = false;
				Destroy (gameObject);
			} else {
				if (_timer > 1.2f) {
					_effectMaster.Teleport ();
				}




				_timer += Time.deltaTime;
			}
		}
	}

	private IEnumerator LaserChargeWait ()
	{
		
		yield return new WaitForSeconds (1.4f);
		if (!_laserChargeFlag) {
			_isShooting = true;
			_timer = 0f;
			_laserEffects.SetActive (true);
			_laserSparksEmitter.enabled = true;
			_laserSmokeEmitter.enabled = true;
			fadingUI.GetComponent <BrianScript> ().isFading = true;
			_laserAudio.Play ();
			_scorchMark.SetActive (true);
			_laserChargeFlag = false;
		}
	}

	private void OnDestroy ()
	{
		Destroy (_scorchMarkClone);
	}
}