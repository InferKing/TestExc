using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum PlayerStatus
{ 
    ToTarget,
    Stay,
    Stop
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LineRenderer _lr;
    [SerializeField] private float _speed;
    private GameObject[] _coinsGO;
    private int coins;
    private List<Touch> _touches;
    private List<Vector2> _position = new List<Vector2>();
    private PlayerStatus _status = PlayerStatus.Stay;
    private float _eps = 0.1f;
    public static Action death, win;
    public static Action<int> getCoin;
    private void OnEnable()
    {
        death += IAmDead;
        win += IAmWin;
    }
    private void OnDisable()
    {
        death -= IAmDead;
        win -= IAmWin;
    }
    private void Start()
    {
        _coinsGO = GameObject.FindGameObjectsWithTag("Coin");
        coins = 0;
    }
    private void Update()
    {
        #region MobileControl
        if (Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {
                _position.Add(Camera.main.ScreenToWorldPoint(touch.position));
            }
            _status = _status is PlayerStatus.ToTarget ? PlayerStatus.ToTarget : PlayerStatus.Stay;
        }
        #endregion
        #region PCControl
        if (Input.GetMouseButtonDown(0))
        {
            _position.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            _status = _status is PlayerStatus.ToTarget ? PlayerStatus.ToTarget : PlayerStatus.Stay;
        }
        #endregion
        MoveToPoint();
        DrawLine();
    }

    private void MoveToPoint()
    {
        if (_position.Count > 0)
        {
            if ((_position[0] - (Vector2)transform.position).magnitude < _eps)
            {
                _position.RemoveAt(0);
                _status = _position.Count > 0 ? PlayerStatus.Stay : PlayerStatus.Stop;
            }
            if (_status is PlayerStatus.Stay)
            {
                float coef = (_position[0] - (Vector2)transform.position).magnitude / _speed;
                _status = PlayerStatus.ToTarget;
                _rb.velocity = (_position[0] - (Vector2)transform.position)/coef;
            }
        }
        if (_status is PlayerStatus.Stop)
        {
            _rb.velocity = Vector3.zero;
        }
    }
    private void DrawLine()
    {
        if (_position.Count > 0)
        {
            Vector3[] temp = new Vector3[2] {_position[0], transform.position};
            _lr.SetPositions(temp);
        }
        else
        {
            _lr.SetPositions(new Vector3[2] {Vector3.zero,Vector3.zero});
        }
    }
    private void IAmDead()
    {
        _status = PlayerStatus.Stop;
        _position.Clear();
        DrawLine();
        gameObject.SetActive(false);
    }
    private void IAmWin()
    {
        _status = PlayerStatus.Stop;
        _position.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            coins++;
            if (coins == _coinsGO.Length)
            {
                win?.Invoke();
            }
            getCoin?.Invoke(coins);
            collision.gameObject.SetActive(false); // Destroy(collision.gameObject);
        }
    }
}
