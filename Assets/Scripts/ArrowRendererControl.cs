using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRendererControl : MonoBehaviour
{
    public LineRenderer line;
    public GameObject ArrowFront;
    public GameObject ArrowEnd;
    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, ArrowFront.transform.position);
        line.SetPosition(1, ArrowEnd.transform.position);
    }
}

