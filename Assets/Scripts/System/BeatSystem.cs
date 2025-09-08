using System;
using CriWare;
using UnityEngine;

public class BeatSystem : MonoBehaviour, IBeatSyncListener
{
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }

    public static BeatSystem Instance;
    
    [SerializeField]private SoundManager _soundManager;
    
    public CriAtomExPlayback Playback;

    public CriAtomExBeatSync.Info BgmInfo;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BeatSyncDispatcher.Instance.Register(this);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Playback = _soundManager.PlayBgm();
        if (Playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info))
        {
            BgmInfo = info;
        }
        else
        {
            Debug.LogWarning("PlayBackから情報を取得できません");
        }
    }

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
    }

    public BeatActionType JudgeBeatAction(CriAtomExPlayback playback, float prevBeatTime, float nextBeatTime)
    {
        if (playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info))
        {
            var nowTime = playback.GetTime() / 1000f;
            float secondsPerBeat = 60f / info.bpm / 2;
            float diffPrev = Mathf.Abs(nowTime - prevBeatTime);
            float diffNext = Mathf.Abs(nowTime - nextBeatTime);
            var diff = Mathf.Min(diffPrev, diffNext);
            var greatDiff = secondsPerBeat * 0.2f;
            var goodDiff = secondsPerBeat * 0.4f;
            if (diff < greatDiff)
            {
                return BeatActionType.Great;
            }

            if (diff < goodDiff)
            {
                return BeatActionType.Good;
            }

            return BeatActionType.Bad;
        }

        Debug.Log("PlayBackが取得できません");
        return BeatActionType.None;
    }
    
    public float BeforeOnBeat(CriAtomExPlayback playback, float preparationTime, int beatNum)
    {
        if (playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info))
        {
            var nowTime = playback.GetTime() / 1000f;
            float secondsPerBeat = 60f / info.bpm / 2;
            var targetTime = secondsPerBeat * beatNum;
            var startTime = targetTime - preparationTime;
            var waitTime = startTime - nowTime;
            if (waitTime > 0) return waitTime;
            Debug.Log("指定した拍は過ぎています");
            return 0;
        }

        Debug.Log("playbackが情報を取得できませんでした");
        return 0;
    }
    
    private void OnDisable()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }
}

public enum BeatActionType
{
    Bad,
    Good,
    Great,
    None
}