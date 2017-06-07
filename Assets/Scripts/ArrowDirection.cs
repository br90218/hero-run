using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour {
    public GameObject theOtherController;
    private Vector3 Direction;
    // Update is called once per frame
    void Update()
    {
        Direction = theOtherController.transform.position - transform.position;
        transform.forward = Direction;
    }
}
