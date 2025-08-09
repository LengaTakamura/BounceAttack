using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using CriWare;

public class BeatActionDemo : MonoBehaviour,IBeatSyncListener
{
    [SerializeField] private string _bgmCueName;
    private CriAtomExAcb _criAtomExAcb;
    private CriAtomExPlayer _atomExPlayer;
    private CancellationTokenSource _cts;

    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }

    [SerializeField] private GameObject _prefab;
    [SerializeField]private CriAtomSource _source;
    private void Start()
    {
        Init();
        PlayBgm(_bgmCueName);
        BeatSyncDispatcher.Instance.Register(this);
    }
    private void Init()
    {
        _cts = new CancellationTokenSource();
    }
    
    private void PlayBgm(string bgmCueName)
    {
        _source.Play(bgmCueName);
    }
    
    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
    
    private void OnDisable()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }

    public void OnBeat()
    {
        Instantiate(_prefab, transform);
    }
}

public interface IBeatSyncListener
{
    void OnBeat();
    
    int CurrentBpm { get; set;}
    
    int DiffBpm { get; set;}
    
    bool IsBeating { get; set;}
    
    
}