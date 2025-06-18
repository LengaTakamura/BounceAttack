using System;
using UnityEngine;

public class PlayerMoveDemo : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField]
    private float _rayCastMaxDistance;

    private bool _isGround;
    [SerializeField] private float _speed;
    [SerializeField]private LayerMask _groundLayer;
    [SerializeField]private float _jumpForce;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _isGround = IsGround();
        Jump();
    }

    private void FixedUpdate()
    {
        Run();
    }

    void Run()
    {
        if (!_isGround) return;
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _rb.linearVelocity = input.normalized * _speed;
    }

    void Jump()
    {
        if (_isGround && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGround()
    {
        var hit = Physics.Raycast(transform.position, Vector3.down,_rayCastMaxDistance, _groundLayer);
        return hit;
    }
}
