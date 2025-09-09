using System;
using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BeatSystem : MonoBehaviour, IBeatSyncListener
{
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    
    [SerializeField]private SoundManager _soundManager;
    
    private CriAtomExPlayback _playback;

    public Action<BeatInfo> OnBeatAction {get; set; }

    private int _count;

    private float _prevTime;
    
    private void Awake()
    {
        BeatSyncDispatcher.Instance.Register(this);
    }

    private void Start()
    {
        Init().Forget();
        _count = -1;
    }

    private async UniTaskVoid Init()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5f));
        _playback = _soundManager.PlayBgm();
        if (!_playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info))
        {
            Debug.LogWarning("PlayBackから情報を取得できません");
        }
    }

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        var beatInfo = UpdateInfo(info);
        OnBeatAction?.Invoke(beatInfo);
    }
    private BeatInfo UpdateInfo(CriAtomExBeatSync.Info info)
    {
        _count++;
        var copy = new BeatInfo
        {
            Bpm = info.bpm,
            SecondsPerBeat = 60f / info.bpm,
            BeatCount = info.beatCount,
            CurrentBeat = _count,
            NowTime = (ulong)_playback.GetTime() / (ulong)1000f
        };
        return copy;
    }
    
    private void OnDisable()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }
}
public struct BeatInfo
{
    public float Bpm;
    public float SecondsPerBeat;
    public float BeatCount;
    public float CurrentBeat;
    public ulong NowTime;
}