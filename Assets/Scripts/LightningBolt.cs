using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningBolt : MonoBehaviour
{
    [SerializeField] private GameObject _startPos;
    [SerializeField] private GameObject _endPos;
    [SerializeField] private int _foldsEachSide;
    [SerializeField] private float _updateInterval;

    public GameObject StartPos
    {
        get { return _startPos; }
        set { _startPos = value; }
    }

    public GameObject EndPos
    {
        get { return _endPos; }
        set { _endPos = value; }
    }

    public int FoldsEachSide
    {
        get { return _foldsEachSide; }
        set { _foldsEachSide = value; }
    }

    public float UpdateInterval
    {
        get { return _updateInterval; }
        set { _updateInterval = value; }
    }

    private LineRenderer _bolt;
    private List<Vector3> _positions;

    // Use this for initialization
    private void Start()
    {
        _bolt = GetComponent<LineRenderer>();
        _bolt.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_bolt == null)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            InitializeBolt();
            _bolt.positionCount = _positions.Count;
            _bolt.SetPositions(_positions.ToArray());
            StartCoroutine("FireBolt");
        }
    }

    private void InitializeBolt()
    {
        _positions = new List<Vector3>();
        _positions.Add(_startPos.transform.position);
        for (var i = 0; i < _foldsEachSide * 2; i++)
        {
            _positions.Add(_startPos.transform.position + (_endPos.transform.position - _startPos.transform.position) /
                           (_foldsEachSide * 2 + 1) * (i + 1));
        }
        _positions.Add(_endPos.transform.position);
    }

    private IEnumerator FireBolt()
    {
        _bolt.enabled = true;
        while (Input.GetButton("Fire1"))
        {
            TwitchBolt();
            yield return new WaitForSeconds(_updateInterval);
        }
        _bolt.enabled = false;
    }

    private void TwitchBolt()
    {
        //Updates the starting point of the bolt
        _bolt.SetPosition(0, _startPos.transform.position);

        //Updates the midpoint (curvepoint) of the bolt using Camera direction
        var curvePoint = _startPos.transform.position + Camera.main.transform.forward *
                         (Vector3.Distance(_startPos.transform.position, _endPos.transform.position) * 0.5f);
        Vector3 newPos;
        for (var i = 1; i < _positions.Count - 1; i++)
        {
            if (i < (_positions.Count / 2))
            {
                newPos = _startPos.transform.position + (curvePoint - _startPos.transform.position) /
                         (_foldsEachSide + 1) * (i + 1) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),
                             Random.Range(-1f, 1f));
            }
            else if (i == _positions.Count / 2)
            {
                newPos = curvePoint + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            else
            {
                newPos = curvePoint + (_endPos.transform.position - curvePoint) /
                         (_foldsEachSide + 1) * (i + 1) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),
                             Random.Range(-1f, 1f));
            }
            _bolt.SetPosition(i, newPos);
        }
    }

}