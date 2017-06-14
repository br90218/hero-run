using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {
    Animator anim;
    public float life;
    public bool de = false;
	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponent<Animator>();
        GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {
        if(anim.GetCurrentAnimatorStateInfo(0).length <= anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            Destroy(gameObject);
        }
	}
}
