using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
public class LightningBolt : MonoBehaviour {
    [SerializeField] private GameObject _startPos;
    [SerializeField] private GameObject _endPos;
    [SerializeField] private int _segment;
    [SerializeField] private float _boltSpeed;

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

    public int Segment
    {
        get { return _segment; }
        set { _segment = value; }
    }

    public float BoltSpeed
    {
        get { return _boltSpeed; }
        set { _boltSpeed = value; }
    }

    private LineRenderer _bolt;
    private Camera _camera;
    private List<Vector3> _positions;

    // Use this for initialization
    private void Start()
    {
        _bolt = GetComponent<LineRenderer>();
        _camera = Camera.main;
        _bolt.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_bolt == null)
        {
            return;
        }
        if (_camera == null)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            _bolt.positionCount = _segment;
            StartCoroutine(DrawBolt());
        }
    }

    private IEnumerator DrawBolt()
    {
        _bolt.enabled = true;
        while (Input.GetButton("Fire1"))
        {
            var angleRad = Vector3.Angle(_endPos.transform.position - _startPos.transform.position,
                               _camera.transform.forward) * Mathf.Deg2Rad;
            var aCos = Mathf.Acos(angleRad);

            var curvePoint = _startPos.transform.position +
                             Vector3.Distance(_startPos.transform.position, _endPos.transform.position) / 2 * aCos *
                             _camera.transform.forward;


            for (var i = 1; i <= _segment; i++)
            {
                var t = i / (float) _segment;
                var point = CalculateBezierPoint(t, _startPos.transform.position, curvePoint,
                    _endPos.transform.position);



                _bolt.SetPosition(i - 1, point);
            }

            var offset = Time.time * _boltSpeed;
            _bolt.material.mainTextureOffset = new Vector2( - offset, 0);

            yield return new WaitForFixedUpdate();
        }
        _bolt.enabled = false;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
}