using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using UnityEngine;

public class JumpingEnemy : EnemyBase, ITracking
{
    private CancellationTokenSource _cts;
    [SerializeField, Label("攻撃の有効範囲")] private int _radius;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private int _delay = 10;
    [SerializeField] private float _moveSpeed = 5f;
    private Func<Vector3> _targetPosition;
    public override void Init(BeatInfo beatinfo)
    {
        base.Init(beatinfo);
        LifeTime(beatinfo.SecondsPerBeat).Forget();
    }

    private async UniTaskVoid LifeTime(float secondsPerBeat)
    {
        _cts = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(secondsPerBeat * _delay), cancellationToken: _cts.Token);
        Suicide();
    }

    public override void EnemyOnBeat(BeatInfo info)
    {
        base.EnemyOnBeat(info);
        var targetPos = _targetPosition != null ? _targetPosition.Invoke() : transform.position;
        var moveVect = (targetPos - transform.position).normalized;
        moveVect.y = 0;
        transform.rotation = Quaternion.LookRotation(moveVect);
        transform.DOJump(transform.position + moveVect * _moveSpeed, jumpPower: 3f, numJumps: 1, duration: info.SecondsPerBeat * 0.8f); // 演出のためのジャンプ
        CheckPlayer();
    }

    private void CheckPlayer()
    {
        var array = new Collider[50];
        var count = Physics.OverlapSphereNonAlloc(transform.position, _radius, array, _playerLayerMask);
        if (count == 0) return;
        for (int i = 0; i < count; i++)
        {
            if (!array[i].gameObject.TryGetComponent(out PlayerManager player)) continue;
            OnAttack(player);
            break;
        }
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void SetTargetPosition(Func<Vector3> targetPositionProvider)
    {
        _targetPosition = targetPositionProvider;
    }
}
