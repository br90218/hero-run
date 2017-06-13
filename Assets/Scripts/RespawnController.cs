using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{

	[SerializeField] private Transform _player;
	[SerializeField] private Transform[] _respawnPositions;
	[SerializeField] private Vector3 _positionOffset;

	private int _checkpointFlag;
	private char _cheat;

	private void Start ()
	{
		_checkpointFlag = 0;
	}

	// Update is called once per frame
	void Update ()
	{
		/*if (Input.GetKeyDown (KeyCode.R)) {
			Player.position = transform.position;
			Player.position += new Vector3 (0, 10, 0);
		} else if (Input.GetKeyDown (KeyCode.F)) {
			transform.position = Player.transform.position;
		} else if (Input.GetKeyDown (KeyCode.Alpha1)) {
			Player.transform.position = TRS1.transform.position;
			Player.transform.position += new Vector3 (0, offset, 0);
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			Player.transform.position = TRS2.transform.position;
			Player.transform.position += new Vector3 (0, offset, 0);
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			Player.transform.position = TRS3.transform.position;
			Player.transform.position += new Vector3 (0, offset, 0);
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			Player.transform.position = TRS4.transform.position;
			Player.transform.position += new Vector3 (0, offset, 0);
		} else if (Input.GetKeyDown (KeyCode.B)) {
			Player.transform.position = Begin.transform.position;
			Player.transform.position += new Vector3 (0, 5, 0);
		}*/
	}

	public void Respawn ()
	{
		if (_cheat == null) {
		
			_player.gameObject.GetComponent<PCPlayerControl> ().Respawn ();
			return;
		}

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
