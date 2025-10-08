using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SphereEnemy : EnemyBase
{
    private int _count;

    [SerializeField] private GameObject _effect; 
    

    private CancellationTokenSource _cts;
    public override void EnemyOnBeat(BeatInfo info)
    {
        base.EnemyOnBeat(info);
    }

    private async UniTaskVoid StartMoving(double time,float preparationTime)
    {
        _cts = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: _cts.Token);
        await transform.DOJump(new Vector3(0f, 0f, 0f), jumpPower: 3f, numJumps: 3, duration: preparationTime).ToUniTask(cancellationToken: _cts.Token);
        Instantiate(_effect, transform.position, Quaternion.identity);
        Kill();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public override void Init(BeatInfo beatinfo)
    {
        base.Init(beatinfo);
        var time = BeatUtility.TimeUntilBeat(beatinfo, 3f, 5);
        StartMoving(time,3f).Forget();
    }
}