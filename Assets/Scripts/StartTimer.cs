using UnityEngine;

public class StartTimer : MonoBehaviour
{
	public GameObject textUI;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "PCPlayer") {
			textUI.GetComponent<TimerUI> ().timerOn = true;
		}
	}
}
