using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{

	[SerializeField] private Transform _player;
	[SerializeField] private Transform[] _respawnPositions;
	[SerializeField] private Vector3 _positionOffset;

	private int _checkpointFlag;

	private void Start ()
	{
		_checkpointFlag = 0;
	}

	// Update is called once per frame
	void Update ()
	{	
		if (Input.GetKey (KeyCode.Alpha0)) {
			_checkpointFlag = 0;
		}
		if (Input.GetKey (KeyCode.Alpha1)) {
			_checkpointFlag = 1;
		}
		if (Input.GetKey (KeyCode.Alpha2)) {
			_checkpointFlag = 2;
		}
		if (Input.GetKey (KeyCode.Alpha3)) {
			_checkpointFlag = 3;
		}
	}

	public void Respawn ()
	{

		switch (_checkpointFlag) {
		case 0:
			_player.position = _respawnPositions [0].position + _positionOffset;
			break;
		case 1:
			_player.position = _respawnPositions [1].position + _positionOffset;
			break;
		case 2:
			_player.position = _respawnPositions [2].position + _positionOffset;
			break;
		case 3:
			_player.position = _respawnPositions [3].position + _positionOffset;
			break;
		default:
			Debug.LogError ("Something went wrong with checkpoint flag: " + _checkpointFlag);
			return;
		}
		_player.gameObject.GetComponent<PCPlayerControl> ().Respawn ();
	}

	public void SetCheckpointFlag (int other)
	{
		_checkpointFlag = other;
	}
}
