using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMoveDemo : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _rayCastMaxDistance;

    private bool _isGround;
    [SerializeField] private float _speed;
    [SerializeField]private LayerMask _groundLayer;
    private int _jumpCount;
    [SerializeField]float[] _jumpForceList ;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        IsGround();
        Jump();
    }

    private void FixedUpdate()
    {
        Run();
        Debug.Log(_rigidbody.linearVelocity);
    }

    void Run()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")).normalized;
        var velo = new Vector3(input.x * _speed, _rigidbody.linearVelocity.y, input.z * _speed);
        _rigidbody.linearVelocity = velo;
    }

    void Jump()
    {
        if (_jumpCount < _jumpForceList.Length && Input.GetKeyDown(KeyCode.Space))
        {
            _jumpCount++;
            var velo = _rigidbody.linearVelocity;
            velo.y = 0;
            _rigidbody.linearVelocity = velo;
            _rigidbody.AddForce(transform.up * SetJumpForce(_jumpCount), ForceMode.Impulse);
        }
    }

    void IsGround()
    {
        var hit = Physics.Raycast(transform.position, Vector3.down,_rayCastMaxDistance, _groundLayer);
        if (hit)
        {
            _isGround = true;
            _jumpCount = 0;
        }
        else
        {
            _isGround = false;
        }
    }

    float SetJumpForce(int jumpCount)
    {
        return _jumpForceList[jumpCount - 1];
    }
}
