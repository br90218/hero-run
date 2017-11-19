using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
	public GameObject m_bullet;
	public float m_speed = 1;
	public float m_turn = 1;
	public float m_angle = 90;
	public float m_Cos;
	public GameObject m_VRPlayer;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_Cos = Mathf.Cos (Mathf.Deg2Rad * m_angle);
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.Keypad8)) {
			m_speed += 1f;
		} else if (Input.GetKeyDown (KeyCode.Keypad8)) {
			m_speed += 0.1f;
		}
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.Keypad2)) {
			m_speed -= 1f;
		} else if (Input.GetKeyDown (KeyCode.Keypad2)) {
			m_speed -= 0.1f;
		}
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.Keypad6)) {
			m_turn += 0.1f;
		} else if (Input.GetKeyDown (KeyCode.Keypad6)) {
			m_turn += 0.01f;
		}
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.Keypad4)) {
			m_turn -= 0.1f;
		} else if (Input.GetKeyDown (KeyCode.Keypad4)) {
			m_turn -= 0.01f;
		}
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.Keypad9)) {
			m_angle += 10f;
		} else if (Input.GetKeyDown (KeyCode.Keypad9)) {
			m_angle += 1f;
		}
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.Keypad7)) {
			m_angle -= 10f;
		} else if (Input.GetKeyDown (KeyCode.Keypad7)) {
			m_angle -= 1f;
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			GameObject sword = Instantiate (m_bullet, transform.position, transform.transform.rotation);
			sword.GetComponent<SwordSontroller> ().m_speed = m_speed;
			sword.GetComponent<SwordSontroller> ().m_turn = m_turn;
			sword.GetComponent<SwordSontroller> ().m_angle = m_angle;
			sword.GetComponent<SwordSontroller> ().m_target = m_VRPlayer;
		}	
	}
}
