
using System;
using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BeatSystem : MonoBehaviour, IBeatSyncListener
{
    [SerializeField] private SoundManager _soundManager;
    
    private CriAtomExPlayback _playback;
    
    private int _count;

    private float _prevTime;
    
    public bool IsPlaying => _playback.status == CriAtomExPlayback.Status.Playing;
    
    public float SecondsPerBeat => 60f / _defaultBpm;
    
    [SerializeField] private int _defaultBpm = 60;
    private void Start()
    {
        BeatSyncDispatcher.Instance.Register(this);
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

    public void OnBeat(BeatInfo info)
    {
    }
    public BeatInfo UpdateInfo(CriAtomExBeatSync.Info info)
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