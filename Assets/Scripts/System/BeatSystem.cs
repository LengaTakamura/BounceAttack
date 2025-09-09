using System;
using CriWare;
using UnityEngine;

public class BeatSystem : MonoBehaviour, IBeatSyncListener
{
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    
    [SerializeField]private SoundManager _soundManager;
    
    private CriAtomExPlayback _playback;

    public Action<BeatInfo> OnBeatAction {get; set; }
    
    private void Awake()
    {
        BeatSyncDispatcher.Instance.Register(this);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _playback = _soundManager.PlayBgm();
        if (!_playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info))
        {
            Debug.LogWarning("PlayBackから情報を取得できません");
        }
    }

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        OnBeatAction?.Invoke(UpdateInfo(info));
    }
    private BeatInfo UpdateInfo(CriAtomExBeatSync.Info info)
    {
        var copy = new BeatInfo
        {
            Bpm = info.bpm,
            SecondsPerBeat = 60f / info.bpm / 2,
            BeatCount = info.beatCount + 1,
            NowTime = _playback.GetTime() / 1000f
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
    public float NowTime;
}