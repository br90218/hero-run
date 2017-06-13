using UnityEngine;

public class PCPlayerControl : MonoBehaviour
{
	[SerializeField] private RespawnController _respawnController;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Death ()
	{
		//play animations here
		_respawnController.Respawn ();
	}

	public void Respawn ()
	{
		
	}
}
