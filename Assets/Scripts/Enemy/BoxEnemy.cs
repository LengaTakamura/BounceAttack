using CriWare;
using DG.Tweening;
using UnityEngine;

public class BoxEnemy : EnemyBase
{
    private int _count;
    public override void EnemyOnBeat(BeatInfo info)
    {
        base.EnemyOnBeat(info);
        Debug.Log("EnemyONBeat");
        _count++;
        if (_count % 2 == 0)
        {
            gameObject.transform.DOScale(new Vector3(3f, 3f, 3f), BeatInfo.SecondsPerBeat).OnComplete(OnComp);
        }
    }

    private void OnComp()
    {
        gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), BeatInfo.SecondsPerBeat);
    }


}
