using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BoxEnemy : EnemyBase
{
    private int _count;
    
    private Tween _tween;
    
    private Sequence _seq;
    public override void Init(BeatInfo beatinfo)
    {
        Death().Forget();
        _count = -1;
    }

    private async UniTaskVoid Death()
    {
        var randomx = Random.Range(0f, 10f);
        var randomy = Random.Range(0f, 10f);
        var randomz = Random.Range(0f, 10f);
        var cts = new CancellationTokenSource();
        await transform.DOLocalMove(new Vector3(randomx, randomy, randomz), 10f).SetEase(Ease.Linear).ToUniTask(cancellationToken: cts.Token);
        Kill();
        cts.Cancel();
        cts.Dispose();
    }

    

    public override void EnemyOnBeat(BeatInfo info)
    {
        base.EnemyOnBeat(info);
        _count++;
        if (_count % 2 == 0)
        {
            _seq?.Kill();
            _seq = DOTween.Sequence()
                .Append(transform.DOScale(new Vector3(3f, 3f, 3f), BeatInfo.SecondsPerBeat))
                .Append(transform.DOScale(new Vector3(1f, 1f, 1f), BeatInfo.SecondsPerBeat));
        }
    }
    


}
