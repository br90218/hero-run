using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour {
    public GameObject flame1, flame2;
	// Use this for initialization
	void Start () {
        if (flame1 != null)
            flame1.SetActive(false);
        if (flame2 != null)
            flame2.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PCPlayer")
        {
            CheckPointActive();
        }
    }

    void CheckPointActive()
    {
        if (flame1 != null)
        {
            flame1.SetActive(true);
            flame1.GetComponent<ParticleSystem>().Play();
        }

        if (flame2 != null)
        {
            flame2.SetActive(true);
            flame2.GetComponent<ParticleSystem>().Play();
        }
    }

}
