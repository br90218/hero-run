using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ResultsBehaviour : MonoBehaviour
{

	[SerializeField] private Text Results;
	[SerializeField] private Text Player1;
	[SerializeField] private Text Player1Score;
	[SerializeField] private Text Player2;
	[SerializeField] private Text Player2Score;
	[SerializeField] private GameObject proceedButton;
	[SerializeField] private VideoPlayer _player;




	private string _PCPlayerName;
	private string _VRPlayerName;
	private float _PCPlayerTime;
	private float _VRPlayerTime;

	// Use this for initialization
	void Start ()
	{
		_player.Prepare ();
		//print (_player.isPrepared);
		_player.Play ();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		_PCPlayerName = PlayerPrefs.GetString ("PCName");
		_VRPlayerName = PlayerPrefs.GetString ("VRName");
		_PCPlayerTime = PlayerPrefs.GetFloat ("PCPlayerScore");
		_VRPlayerTime = PlayerPrefs.GetFloat ("VRPlayerScore");
		StartDisplaying ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		print (Cursor.visible);
		print (_player.isPrepared + " " + _player.isPlaying);
		if (_player.isPrepared) {
			if (!_player.isPlaying) {
				_player.Play ();
			}
		}
	}

	private void StartDisplaying ()
	{
		Results.enabled = true;
		StartCoroutine ("DisplayPlayer1");
	}

	private IEnumerator DisplayPlayer1 ()
	{
		Player1.text = _PCPlayerName + " broke through the brutal defenses\nof " + _VRPlayerName + " within:";
		var _timer = 0f;
		Player1.enabled = true;
		Player1.color = Color.clear;
		while (_timer < 1f) {
			
			Player1.color = Color.Lerp (Color.clear, Color.white, _timer);
			_timer += Time.deltaTime;
			yield return new WaitForFixedUpdate ();
		}
		yield return new WaitForSeconds (1f);
		StartCoroutine ("DisplayPlayer1Score");
	}

	private IEnumerator DisplayPlayer1Score ()
	{
		var min = _PCPlayerTime / 60;
		var sec = (_PCPlayerTime % 60);
		var msec = ((_PCPlayerTime * 100) % 100);
		var _timer = 0f;
		Player1Score.enabled = true;
		Player1Score.text = "00:00:00";
		yield return new WaitForSeconds (1f);
		while (_timer < 0.5f) {
			var smin = (int)Mathf.Lerp (0f, min, _timer * 2);
			var ssec = (int)Mathf.Lerp (0f, sec, _timer * 2);
			var smsec = (int)Mathf.Lerp (0f, msec, _timer * 2);
			Player1Score.text = smin.ToString ("00") + ":" + ssec.ToString ("00") + ":" + smsec.ToString ("00");
			_timer += Time.deltaTime;
			yield return new WaitForFixedUpdate ();
		}
		yield return new WaitForSeconds (2f);
		StartCoroutine ("DisplayPlayer2");
	}


	private IEnumerator DisplayPlayer2 ()
	{
		if (_PCPlayerTime < _VRPlayerTime) {
			Player2.text = "Having tried as hard as possible, " + _VRPlayerName + "\nreached the heavenly gates of";
		} else {
			Player2.text = "However, " + _VRPlayerName + " outran " + _PCPlayerName + "\nwith a time of";
		}

		var _timer = 0f;
		Player2.enabled = true;
		Player2.color = Color.clear;
		while (_timer < 1f) {

			Player2.color = Color.Lerp (Color.clear, Color.white, _timer);
			_timer += Time.deltaTime;
			yield return new WaitForFixedUpdate ();
		}
		yield return new WaitForSeconds (1f);
		StartCoroutine ("DisplayPlayer2Score");
	}

	private IEnumerator DisplayPlayer2Score ()
	{
		var min = _VRPlayerTime / 60;
		var sec = (_VRPlayerTime % 60);
		var msec = ((_VRPlayerTime * 100) % 100);
		var _timer = 0f;
		Player2Score.enabled = true;
		Player2Score.text = "00:00:00";
		yield return new WaitForSeconds (1f);
		while (_timer < 0.5f) {
			var smin = (int)Mathf.Lerp (0f, min, _timer * 2);
			var ssec = (int)Mathf.Lerp (0f, sec, _timer * 2);
			var smsec = (int)Mathf.Lerp (0f, msec, _timer * 2);
			Player2Score.text = smin.ToString ("00") + ":" + ssec.ToString ("00") + ":" + smsec.ToString ("00");
			_timer += Time.deltaTime;
			yield return new WaitForFixedUpdate ();
		}
		yield return new WaitForSeconds (2f);
		proceedButton.SetActive (true);
	}


	public void OnContinue ()
	{
		SceneManager.LoadScene ("Title");
	}
}
