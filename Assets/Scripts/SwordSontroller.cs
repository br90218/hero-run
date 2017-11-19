using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSontroller : MonoBehaviour
{
	public float m_speed = 1;
	public GameObject m_target;
	public float m_turn = 0.5f;
	public Vector3 m_velocity;
	public Vector3 m_dir;
	public int auto = 1;
	public float m_distance;
	public float m_angle = 90;
	public float m_COS;
	// Use this for initialization
	void Start ()
	{
		auto = 1;
		m_velocity = transform.forward * m_speed;
		m_distance = Vector3.Distance (transform.position, m_target.transform.position);        
	}
	
	// Update is called once per frame
	void Update ()
	{
		float distance = Vector3.Distance (transform.position, m_target.transform.position);
		transform.position += m_velocity * Time.deltaTime * m_speed;
		m_velocity += (m_target.transform.position - transform.position) * m_turn * auto;
		m_COS = Mathf.Cos (Mathf.Deg2Rad * m_angle);
		if (Vector3.Dot (transform.forward, m_target.transform.position - transform.position) < Mathf.Cos (Mathf.Deg2Rad * m_angle)) {
			auto = 0;
		}
            
		m_velocity.Normalize ();
		m_velocity *= m_speed;
		m_dir = transform.forward;

	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.gameObject == m_target) {
			print ("HIT!");
			Destroy (gameObject);
		}
	}
}
