using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderController : MonoBehaviour {
    public GameObject player;
    public float delay, force, radius;
    public bool b;
    Vector3 pos;
	// Use this for initialization
	void Start () {
        delay = .5f;
        b = false;
        force = 1000;
        radius = 10;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.E))
        {
            pos = player.transform.position - (new Vector3(0, 1, 0));
            player.GetComponent<Rigidbody>().AddExplosionForce(force, pos, radius);
            if (b == false)
            {
                b = true;
                delay = 0f;
                pos = player.transform.position - (new Vector3(0, 1, 0));
            }
        }
        if(b == true)
        {
            //delay -= Time.deltaTime;
            if(delay <= 0)
            {
                b = false;
                player.GetComponent<Rigidbody>().AddExplosionForce(force, pos, radius);
            }
        }
	}
}
