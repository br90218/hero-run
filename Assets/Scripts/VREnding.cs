using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VREnding : MonoBehaviour {
    public GameObject imageObj;
    public Image image;
    public float duration, timer;
    public bool isEnd;
    // Use this for initialization
    void Start () {
        image = imageObj.GetComponent<Image>();
        isEnd = false;
        timer = 0;
        duration = 2;
	}
	
	// Update is called once per frame
	void Update () {
		if(isEnd)
        {
            timer += Time.deltaTime;
            if(timer <= duration)
            {
                float alpha = 1 - (1 - (timer / duration)) * (1 - (timer / duration));
                image.color = new Color(1, 1, 1, alpha);
            }
            else
            {
                isEnd = false;
            }
        }
        else
        {
            timer = 0;
        }
	}
}
