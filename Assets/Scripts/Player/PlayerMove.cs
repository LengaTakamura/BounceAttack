using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _rayCastMaxDistance;

    [SerializeField] private float _speed;
    [SerializeField]private LayerMask _groundLayer;
    private int _jumpCount;
    [SerializeField]float[] _jumpForceList ;
    [SerializeField] private bool _canBlink;
    [SerializeField] private float _blinkPower;
    [SerializeField] private float _blinkCoolTime;
    [SerializeField] private float _blinkTime;
    private CancellationTokenSource _cts;
    private InputManager _inputManager;

    private bool _isBlinking;

    public bool IsBlinking { get { return _isBlinking;} }
    public void Init(InputManager inputManager)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _canBlink = true;
        _cts = new CancellationTokenSource();
        _inputManager = inputManager;
    }

    public void OnUpdate()
    {
        IsGround();
        Jump();
        Blink();
    }

    public void OnFixedUpdate()
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
            if (_jumpCount >= _jumpForceList.Length)
            {
                _jumpCount = 0;
            }
        }
    }

    private void Blink()
    {
        if (_inputManager.CurrentInputType != InputType.Blink || !_canBlink) return;
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
        _isBlinking = true;
        try
        {
            while (t < 0.8f) // 1.0���ƍŌ㌸������̂ő��x���Ȃ�ׂ��ۂ����܂܏I�����邱�ƂŃL���̂��铮����
            {
                currentTime += Time.deltaTime;
                t = Mathf.Clamp01(currentTime / _blinkTime);
                transform.position = Vector3.Lerp(start, target, t);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: _cts.Token);
            }
        }
        finally
        {
            // finally�ɓ���邱�ƂŁA�L�����Z�������m���Ƀt���O���߂�
            _isBlinking = false;
        }
        _canBlink = true;
    } 
    
    private float SetJumpForce(int jumpCount)
    {
        return _jumpForceList[jumpCount - 1];
    }
}
