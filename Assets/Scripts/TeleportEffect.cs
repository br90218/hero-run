using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : MonoBehaviour
{
	public Transform FromTransform;
	public Transform ToTransform;
	[SerializeField] private GameObject _laserBeam;
	public GameObject fadingUI;
	private GameObject _fromLaser = null;
	private GameObject _toLaser = null;

	public void Activate ()
	{

		if (!_fromLaser) {
			_fromLaser = Instantiate (_laserBeam, FromTransform.position, Quaternion.identity, null);
			_fromLaser.GetComponent<TeleportBeam> ()._effectMaster = this;
			_fromLaser.GetComponent<TeleportBeam> ().fadingUI = fadingUI;
		}
		if (!_toLaser) {
			_toLaser = Instantiate (_laserBeam, ToTransform.position, Quaternion.identity, null);
			_toLaser.GetComponent<TeleportBeam> ()._effectMaster = this;
			_toLaser.GetComponent<TeleportBeam> ().fadingUI = fadingUI;
		}

		_fromLaser.GetComponent<TeleportBeam> ().IsInPosition = true;
		_toLaser.GetComponent<TeleportBeam> ().IsInPosition = true;

	}

	public void Deactivate ()
	{
		if (_fromLaser) {
			_fromLaser.GetComponent<TeleportBeam> ().IsInPosition = false;
			Destroy (_fromLaser);
		}
		if (_toLaser) {
			_toLaser.GetComponent<TeleportBeam> ().IsInPosition = false;
			Destroy (_toLaser);
		}
	}

	public void Teleport ()
	{
		FromTransform.position = ToTransform.position;
		FromTransform.rotation = ToTransform.rotation;
		FromTransform.localScale = ToTransform.localScale;
	}
}
