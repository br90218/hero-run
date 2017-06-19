using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
	public GameObject shakingCam;
	public GameObject playerCam;
	public float force = 5;
	public float travSpeed = 1f;
	public bool isShaking;
	float xu, xv, yu, yv, zu, zv;
	bool isInit = false;
	// Use this for initialization
	void Start ()
	{
		xu = 0;
		xv = 0;
		yu = 0;
		yv = 0;
		zu = 0;
		zv = 0;
		isShaking = false;
	}
	// Update is called once per frame
	void Update ()
	{
        
        
		if (isShaking == false) {
			playerCam.GetComponent<Camera> ().enabled = true;
			shakingCam.GetComponent<Camera> ().enabled = false;

			//camera.transform.position = player.transform.position + offset;                
			return;
		}
		shakingCam.GetComponent<Camera> ().enabled = true;
		playerCam.GetComponent<Camera> ().enabled = false;
/*		xu = (xu <= 1.0) ? xu + 0.01f * travSpeed : 0;
		xv = (xv <= 1.0) ? ((xu == 0) ? xv + 0.1f * travSpeed : xv) : 0;
		yu = (yu <= 1.0) ? ((yv == 0) ? yu + 0.1f * travSpeed : yu) : 0;
		yv = (yv <= 1.0) ? yv + 0.01f * travSpeed : 0;
		Vector3 shakeV = new Vector3 (Mathf.PerlinNoise (xu, xv) - .5f, Mathf.PerlinNoise (yu, yv) - .5f, 0);
		shakingCam.transform.position = playerCam.transform.position + shakeV * force;
		shakingCam.transform.rotation = playerCam.transform.rotation;*/





	}
}
