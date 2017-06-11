using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{

    public float SpinSpeed = 50.0f;
    public Vector3 Axis = Vector3.one;
    //float X, Y, Z;
    [SerializeField]
    Material Skybox;

    public bool rotateSky = false;
    float time;
    void Start()
    {
      //  X = transform.eulerAngles.x;
     //   Y = transform.eulerAngles.y;
      //  Z = transform.eulerAngles.z;
    }
    void FixedUpdate()
    {
        time += Time.deltaTime;
        //transform.eulerAngles = new Vector3(X + time * SpinSpeed * Axis.x, Y + time * SpinSpeed* Axis.y, Z + time * SpinSpeed * Axis.z);
        transform.Rotate(Axis, SpinSpeed, Space.World);
        if (rotateSky && Skybox)
            Skybox.SetFloat("_Rotation", -transform.eulerAngles.y);
    }

}
