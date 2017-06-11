using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]

public class PointLight : MonoBehaviour
{
    GameObject PointLightOBJ;
    //  [SerializeField]
    bool isInVisibleRange = true;
    [SerializeField]
    bool lockColliderRadius = true;
    public bool IsInVisibleRange
    {
        get
        {
            return isInVisibleRange;
        }
        set
        {
            if (value != isInVisibleRange)
                isInVisibleRange = value;

        }

    }
    SphereCollider SphereCollider;
    void OnEnable()
    {
        PointLightOBJ = this.gameObject;
        SphereCollider = PointLightOBJ.GetComponent<SphereCollider>();
        if (!SphereCollider)
            SphereCollider = PointLightOBJ.AddComponent<SphereCollider>();

        PointLightOBJ.transform.localScale = Vector3.one;
        SphereCollider.isTrigger = true;

    }
    void Update()
    {
        if (SphereCollider && lockColliderRadius) SphereCollider.radius = PointLightOBJ.GetComponent<Light>().range;
    }

    public Vector3 GetPointLightPosition
    {
        get
        {
            return PointLightOBJ.transform.position;
        }

    }
    public float GetPointLightRange
    {
        get
        {
            return PointLightOBJ.GetComponent<Light>().range / 5;
        }

    }
    public Color GetPointLightColor
    {
        get
        {
            return PointLightOBJ.GetComponent<Light>().color;
        }

    }

    public float GetPointLightIntensity
    {
        get
        {
            return PointLightOBJ.GetComponent<Light>().intensity;
        }

    }
    public Transform GetTransform
    {
        get
        {
            return PointLightOBJ.transform;
        }
    }
}