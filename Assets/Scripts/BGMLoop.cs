using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class BGMLoop : MonoBehaviour
{
	public AudioClip[] clips = new AudioClip[3];
	private double nextEventTime;
	private int flip = 0;
	private AudioSource[] audioSources = new AudioSource[3];
	private bool running = false;
	private bool _introPlayed = false;

	void Start ()
	{
		int i = 0;
		while (i < 3) {
			GameObject child = new GameObject ("Player");
			child.transform.parent = gameObject.transform;
			audioSources [i] = child.AddComponent<AudioSource> ();
			i++;
		}
		nextEventTime = AudioSettings.dspTime + 2.0F;
		running = true;
	}

	void Update ()
	{
		if (!running)
			return;

		double time = AudioSettings.dspTime;
		if (time + 1.0F > nextEventTime) {
			if (!_introPlayed) {
				audioSources [2].clip = clips [2];
				audioSources [2].PlayScheduled (nextEventTime);
				_introPlayed = true;
				nextEventTime += 129.286f;
				return;
			}
			audioSources [flip].clip = clips [flip];
			audioSources [flip].PlayScheduled (nextEventTime);
			Debug.Log ("Scheduled source " + flip + " to start at time " + nextEventTime);
			nextEventTime += 128.553f;
			flip = 1 - flip;
		}
	}
}