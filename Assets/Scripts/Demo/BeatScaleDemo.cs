using System;
using System.Threading;
using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class BeatScaleDemo : MonoBehaviour,IBeatSyncListener
{
    
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    private int _count;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private GameObject _effectPrefab;
    [SerializeField] private float _effectTimeOffset = 0.1f;
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
            //var waitTime =  BeatUtility.BeforeOnBeat(BeatActionDemo.BGMPlayback, 2, 50,2);
           // PrefabBigger(waitTime).Forget();
        }
    }

    private async UniTaskVoid PrefabBigger(float time)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        var obj =  Instantiate(_prefab,new Vector3(0f,0f,0f),Quaternion.identity);
        await obj.transform.DOScale(new Vector3(3f, 3f, 3f), time + _effectTimeOffset).SetEase(Ease.OutBounce).ToUniTask(cancellationToken: cts.Token);
        cts.Cancel();
        cts.Dispose();
        obj.gameObject.SetActive(false);
        Instantiate(_effectPrefab,obj.transform.position,obj.transform.rotation);
        Debug.Log("OnBeat!!!!!!!!!!!");
    }

    void OnComp()
    {
        gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
    }
    
}
