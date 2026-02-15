
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using UnityEngine;

public class SphereEnemy : EnemyBase, ITracking
{
    private int _count;

    [SerializeField] private GameObject _effect;

    [SerializeField] private int _preparationBeat = 3;

    private CancellationTokenSource _cts;

    [SerializeField, Label("爆撃の有効範囲")] private int _radius;
    
    [SerializeField] private LayerMask _playerLayerMask;

    private Func<Vector3> _targetPosition;
    public override void Init(BeatInfo beatinfo)
    {
        base.Init(beatinfo);
        var preparationTime = beatinfo.SecondsPerBeat * _preparationBeat;
        var time = BeatUtility.TimeUntilBeat(beatinfo, preparationTime, 5);
        StartMoving(time, preparationTime).Forget();
    }

    private async UniTaskVoid StartMoving(double time, float preparationTime)
    {
        _cts = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: _cts.Token);
        var targetPos = _targetPosition != null ? _targetPosition.Invoke() : transform.position;
        await transform.DOJump(targetPos, jumpPower: 3f, numJumps: 3, duration: preparationTime)
            .ToUniTask(cancellationToken: _cts.Token);
        if (_effect)
        {
            Instantiate(_effect, transform.position, Quaternion.identity);
        }
        CheckPlayer();
        Suicide();
    }

    private void CheckPlayer()
    {
        var array = new Collider[50];
        var count = Physics.OverlapSphereNonAlloc(transform.position, _radius,array,_playerLayerMask);
        if(count == 0) return;
        for (int i = 0; i < count; i++)
        {
            if(!array[i].gameObject.TryGetComponent(out PlayerManager player)) continue;
            OnAttack(player);
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