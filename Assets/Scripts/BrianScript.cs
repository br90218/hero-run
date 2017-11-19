using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrianScript : MonoBehaviour
{
	public GameObject rController;
	public GameObject lController;
	public float weak = 1999;
	public bool isFading;
	public float delay, fadingOut = 2, fading = 2.5f, fadingIn = 0.5f;
	public float c0, c1, c2, c3;
	public float d0, d1, d2;
	public Image image;
	bool init = false;
	float forceR, forceL;
	private SteamVR_TrackedObject _trackedObj_Right;
	private SteamVR_TrackedObject _trackedObj_Left;
	// Use this for initialization
	void Start ()
	{
		forceR = forceL = 1000;
		init = false;
		c0 = c1 = c2 = c3 = 0;
		image = gameObject.GetComponent<Image> ();
		isFading = false;
		delay = .8f;
		fadingOut = .2f;
		fading = .4f;
		fadingIn = .4f;
		image.color = new Color (0, 0, 0, 0);
	}

	private void Awake ()
	{
		_trackedObj_Left = lController.GetComponent<SteamVR_TrackedObject> ();
		_trackedObj_Right = rController.GetComponent<SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (isFading) {
			if (init == false) {
				init = true;
				if (_trackedObj_Left.transform.position.y < _trackedObj_Right.transform.position.y) {
					forceL = weak;
					forceR = 3999;
				} else {
					forceL = 3999;
					forceR = weak;
				}
			}
			if (c0 <= delay) {
				c0 += Time.deltaTime;
				SteamVR_Controller.Input ((int)_trackedObj_Right.index).TriggerHapticPulse ((ushort)forceR);
				SteamVR_Controller.Input ((int)_trackedObj_Left.index).TriggerHapticPulse ((ushort)forceL);
			} else if (c1 <= fadingIn) {
				c1 += Time.deltaTime;
				image.color = new Color (1, 1, 1, (c1 / fadingIn));
				SteamVR_Controller.Input ((int)_trackedObj_Right.index).TriggerHapticPulse ((ushort)forceR);
				SteamVR_Controller.Input ((int)_trackedObj_Left.index).TriggerHapticPulse ((ushort)forceL);
			} else if (c2 <= fading) {
				c2 += Time.deltaTime;
				SteamVR_Controller.Input ((int)_trackedObj_Right.index).TriggerHapticPulse ((ushort)forceR);
				SteamVR_Controller.Input ((int)_trackedObj_Left.index).TriggerHapticPulse ((ushort)forceL);
			} else if (c3 <= fadingOut) {
				c3 += Time.deltaTime;
				image.color = new Color (1, 1, 1, 1 - (c3 / fadingOut));
				SteamVR_Controller.Input ((int)_trackedObj_Right.index).TriggerHapticPulse ((ushort)(forceR - (c3 * 5) * forceR));
				SteamVR_Controller.Input ((int)_trackedObj_Left.index).TriggerHapticPulse ((ushort)(forceL - (c3 * 5) * forceL));
			} else {
				isFading = false;
			}

		} else {
			c0 = c1 = c2 = c3 = 0;
		}
	}
}