using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererControl : MonoBehaviour {
    public LineRenderer line;
    public GameObject stringAttachOne;
    public GameObject stringAttachTwo;
    public GameObject midPoint;
    public ArrowShoot AS;

    void Start()
    {

    }
    // Update is called once per frame
    void Update () {
        if (AS.isShot == false)
        {
            line.SetPosition(0, stringAttachOne.transform.position);
            line.SetPosition(1, midPoint.transform.position);
            line.SetPosition(2, stringAttachTwo.transform.position);
        }
        else
        {
            line.SetPosition(0, stringAttachOne.transform.position);
            line.SetPosition(1, stringAttachTwo.transform.position);
        }
    }
}
