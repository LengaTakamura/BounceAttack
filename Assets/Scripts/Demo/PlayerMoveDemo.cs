using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private bool _canBlink;
    [SerializeField] private float _blinkPower;
    [SerializeField] private float _blinkCoolTime;
    [SerializeField] private float _blinkTime;
    private CancellationTokenSource _cts;
    private float _lerpTime;
    private InputManager _inputManager;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _canBlink = true;
        _inputManager = BeatSyncDispatcher.Instance.Get<InputManager>();
        _cts = new CancellationTokenSource();
    }

    private void Update()
    {
        IsGround();
        Jump();
        Blink();
    }

    private void FixedUpdate()
    {
        Run();
    }

    private void Run()
    {
        var input = _inputManager.GetMoveDirection();
        var velo = new Vector3(input.x * _speed, _rigidbody.linearVelocity.y, input.z * _speed);
        _rigidbody.linearVelocity = velo;
    }

    private void Jump()
    {
        if (_jumpCount >= _jumpForceList.Length || _inputManager.CurrentInputType != InputType.Spase) return;
        _jumpCount++;
        var velo = _rigidbody.linearVelocity;
        velo.y = 0;
        _rigidbody.linearVelocity = velo;
        _rigidbody.AddForce(transform.up * SetJumpForce(_jumpCount), ForceMode.Impulse);
    }

    private void IsGround()
    {
        var hit = Physics.Raycast(transform.position, Vector3.down,_rayCastMaxDistance, _groundLayer);
        if (hit)
        {
            _isGround = true;
            if (_jumpCount >= _jumpForceList.Length)
            {
                _jumpCount = 0;
            }
        }
        else
        {
            _isGround = false;
        }
    }

    private void Blink()
    {
        if (_inputManager.CurrentInputType != InputType.Blink || !_canBlink) return;
        _lerpTime = 0;
        var velo = _rigidbody.linearVelocity.normalized;
        velo.y = 0;
        if (velo == Vector3.zero)
        {
            velo = transform.forward;
        }
        _canBlink = false;
        BlinkCoolDown(velo).Forget();
    }

    private async UniTaskVoid BlinkCoolDown(Vector3 velo)
    {
        float currentTime = 0;
        float t = 0;
        var target = transform.position + velo * _blinkPower;
        var start = transform.position;
        while (t < 0.8)
        {
            currentTime += Time.deltaTime;
            t = Mathf.Clamp01(currentTime / _blinkTime);
            transform.position = Vector3.Lerp(start,target, t);
            await UniTask.Yield( PlayerLoopTiming.Update, cancellationToken: _cts.Token);
        }
        _canBlink = true;
    } 

    private float SetJumpForce(int jumpCount)
    {
        return _jumpForceList[jumpCount - 1];
    }
}
