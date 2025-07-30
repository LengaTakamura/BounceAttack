using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using CriWare;

public class BeatActionDemo : MonoBehaviour,IBeatSyncListener
{
    [SerializeField]private string _cueSheetName;
    [SerializeField]private string _acbFilePath;
    [SerializeField] private string _bgmCueName;
    private CriAtomExAcb _criAtomExAcb;
    private CriAtomExPlayer _atomExPlayer;
    private CancellationTokenSource _cts;

    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }

    [SerializeField] private GameObject _prefab;
    private void Start()
    {
        Init().Forget();
        PlayBgm(_bgmCueName);
    }
    private async UniTaskVoid Init()
    {
        _cts = new CancellationTokenSource();

        _atomExPlayer = new CriAtomExPlayer();

        // キューシートの追加
        var cueSheet = CriAtom.AddCueSheet(_cueSheetName, _acbFilePath, "");

        // 非同期でロード完了を待つ
        await UniTask.WaitUntil(() => !cueSheet.IsLoading, cancellationToken: _cts.Token);

        _criAtomExAcb = cueSheet.acb;
    }
    
    private void PlayBgm(string bgmCueName)
    {
        if (!_criAtomExAcb.Exists(bgmCueName)) return;
        _atomExPlayer.SetCue(_criAtomExAcb, bgmCueName);
        _atomExPlayer.Start();
    }
    
    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void OnEnable()
    {
       BeatSyncDispatcher.Instance.Register(this);
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