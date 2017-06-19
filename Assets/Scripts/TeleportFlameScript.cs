using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportFlameScript : MonoBehaviour
{

	public GameObject _trigger;

	private GameObject _nowFire;

	private void Start ()
	{
		_nowFire = Instantiate (_trigger, transform);
	}


	public void Ignite ()
	{
		if (_nowFire) {
			_nowFire.transform.parent = null;
			_nowFire.GetComponent<IgniteFlame> ().StopFlame ();
			Destroy (_nowFire, 3f);
		}
		_nowFire = Instantiate (_trigger, transform);
		_nowFire.GetComponent<IgniteFlame> ().Ignite ();
	}
}
