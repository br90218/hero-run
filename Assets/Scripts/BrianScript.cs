using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrianScript : MonoBehaviour
{
	public bool isFading;
	public float delay, fadingOut = 2, fading = 2.5f, fadingIn = 0.5f;
	public float c0, c1, c2, c3;
	public float d0, d1, d2;
	public Image image;
	// Use this for initialization
	void Start ()
	{
		c0 = c1 = c2 = c3 = 0;
		image = gameObject.GetComponent<Image> ();
		isFading = false;
		delay = .8f;
		fadingOut = .2f;
		fading = .4f;
		fadingIn = .4f;
		image.color = new Color (0, 0, 0, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isFading) {
			if (c0 <= delay) {
				c0 += Time.deltaTime;
			} else if (c1 <= fadingIn) {
				c1 += Time.deltaTime;
				image.color = new Color (1, 1, 1, (c1 / fadingIn));
			} else if (c2 <= fading) {
				c2 += Time.deltaTime;
			} else if (c3 <= fadingOut) {
				c3 += Time.deltaTime;
				image.color = new Color (1, 1, 1, 1 - (c3 / fadingOut));
			} else {
				isFading = false;
			}

		} else {
			c0 = c1 = c2 = c3 = 0;
		}
	}
}