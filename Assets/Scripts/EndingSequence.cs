using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingSequence : MonoBehaviour
{
    public TimerUI TimerUI;

	[SerializeField]
	private Camera _warpedCamera;
	[SerializeField]
	private Image _flash;
	[SerializeField]
	private AnimationCurve _fovCurve;
	[SerializeField]
	private AnimationCurve _spriteAlphaCurve;
	[SerializeField]
	private VREnding _vrEnding;

	// Update is called once per frame
	void Update ()
	{

    }

	private void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("PCPlayer")) {
			_vrEnding.isEnd = true;
			StartEnding ();
		}
    }

	private void StartEnding ()
	{
        SaveCurrentScore();
		StartCoroutine ("WarpCamera");
	}

	private IEnumerator WarpCamera ()
	{
		var _timer = 0f;
		while (_timer < _fovCurve [_fovCurve.length - 1].time) {
			_warpedCamera.fieldOfView = _fovCurve.Evaluate (_timer);

			_flash.color = new Color (1f, 1f, 1f, _spriteAlphaCurve.Evaluate (_timer));

			_timer += Time.deltaTime;
			yield return new WaitForFixedUpdate ();
		}
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

    private void SaveCurrentScore()
    {
        PlayerPrefs.SetFloat("Current Player Score", TimerUI.timer);
    }
}
