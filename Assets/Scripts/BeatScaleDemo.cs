using System;
using System.Threading;
using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
public class BeatScaleDemo : MonoBehaviour,IBeatSyncListener
{
    
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    private int _count;
    [SerializeField] private GameObject _prefab;
    void Start()
    {
        BeatSyncDispatcher.Instance.Register(this);
    }
    
    private void OnDisable()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        _count++;
        if (_count % 2 == 0)
        {
            gameObject.transform.DOScale(new Vector3(3f, 3f, 3f), 0.5f).OnComplete(OnComp);
        }

        if (_count == 4)
        {
            var waitTime =  BeatSyncDispatcher.Instance.BeforeOnBeat(BeatActionDemo.BGMPlayback, 2, 50);
            Debug.Log(waitTime);
            PrefabBigger(waitTime).Forget();
        }
    }

    private async UniTaskVoid PrefabBigger(float time)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        var obj =  Instantiate(_prefab);
        await obj.transform.DOScale(new Vector3(3f, 3f, 3f), time).SetEase(Ease.OutBounce).ToUniTask(cancellationToken: cts.Token);
        cts.Cancel();
        cts.Dispose();
        obj.gameObject.SetActive(false);
        Debug.Log("OnBeat!!!!!!!!!!!");
    }

    void OnComp()
    {
        gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
    }


}
