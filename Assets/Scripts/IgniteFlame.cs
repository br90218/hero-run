using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteFlame : MonoBehaviour
{

	[SerializeField] private ParticleSystem[] _flames;

	public void Ignite ()
	{
		foreach (ParticleSystem system in _flames) {
			system.Play ();
		}
	}

	public void StopFlame ()
	{
		foreach (ParticleSystem system in _flames) {
			system.Stop ();
		}
	}
}
