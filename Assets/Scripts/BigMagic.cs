using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMagic : MonoBehaviour {
    public GameObject SpawnObj;
    public GameObject Target;
    public float spawnRadius;
    private Vector3[] spawnPosition = new Vector3[100];
    private WaitForSeconds wait = new WaitForSeconds(0.03f);
    public SteamVR_TrackedObject Controller;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var device = SteamVR_Controller.Input((int)Controller.index);
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            StartCoroutine(spawnObjs());
        }
    }

    IEnumerator spawnObjs()
    {
        for (int i = 0; i < 100; i++)
        {
            spawnPosition[i] = new Vector3(Random.insideUnitSphere.x * spawnRadius, Random.insideUnitSphere.y * spawnRadius, Random.insideUnitSphere.z * spawnRadius);
            GameObject obj = (GameObject)Instantiate(SpawnObj, spawnPosition[i] + transform.position, Quaternion.identity);
            obj.GetComponent<TrackingControl>().Target = GameObject.FindGameObjectsWithTag("Player")[0];
            yield return wait;
        }
    }
}
