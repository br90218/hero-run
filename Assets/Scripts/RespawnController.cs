using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour {
    public GameObject Player;
    public GameObject TRS1;
    public GameObject TRS2;
    public GameObject TRS3;
    public GameObject TRS4;
    public GameObject Begin;
    int offset = 25;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Player.transform.position = transform.position ;
            Player.transform.position += new Vector3(0, 10, 0);
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            transform.position = Player.transform.position;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Player.transform.position = TRS1.transform.position;
            Player.transform.position += new Vector3(0, offset, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Player.transform.position = TRS2.transform.position;
            Player.transform.position += new Vector3(0, offset, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Player.transform.position = TRS3.transform.position;
            Player.transform.position += new Vector3(0, offset, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Player.transform.position = TRS4.transform.position;
            Player.transform.position += new Vector3(0, offset, 0);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            Player.transform.position = Begin.transform.position;
            Player.transform.position += new Vector3(0, 5, 0);
        }
    }
}
