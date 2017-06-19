using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUI : MonoBehaviour
{

	[ColorUsageAttribute (true, true, 0f, 10f, 0.125f, 3f)]
	public Color timerOnColor;
	public bool timerOn = false;
	TextMesh textMesh;
	public float timer;
	// Use this for initialization
	void Start ()
	{
		timer = 0;
		textMesh = gameObject.GetComponent<TextMesh> ();
		textMesh.text = "00:00:00";
	}

	// Update is called once per frame
	void Update ()
	{
		if (timerOn == false)
			return;
		textMesh.color = timerOnColor;
		timer += Time.deltaTime;
		string min = Mathf.Floor (timer / 60).ToString ("00");
		string sec = (timer % 60).ToString ("00");
		string minisec = (timer * 100 % 100).ToString ("00");
		textMesh.text = min + ":" + sec + ":" + minisec;
	}

	public void SetTimerUI (float tim)
	{
		timer = tim;
		string min = Mathf.Floor (timer / 60).ToString ("00");
		string sec = (timer % 60).ToString ("00");
		string minisec = (timer * 100 % 100).ToString ("00");
		textMesh.text = min + ":" + sec + ":" + minisec;
	}
}
