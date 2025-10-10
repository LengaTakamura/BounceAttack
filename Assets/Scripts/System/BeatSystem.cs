using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace System
{
    public class BeatSystem : MonoBehaviour, IBeatSyncListener
    {
        [SerializeField] private SoundManager _soundManager;

        private CriAtomExPlayback _playback;

        private int _count;

        private float _prevTime;

        public float DefaultSecondsPerBeat => 60f / _defaultBpm;

        [SerializeField] private int _defaultBpm = 60;

        public TempoState CurrentTempo { get; private set; }

        public int ChangeTempoBeat { get; private set; } = 50;
        public int PrepareBeat { get; private set; } = 5;
        public int BetweenBeats { get; private set; } = 5;
        
        [SerializeField] private float _waitingTime = 5f;

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
            CurrentTempo = TempoState.None;
            PrepareBeat = 5;
            BetweenBeats = 5;
            await UniTask.Delay(TimeSpan.FromSeconds(_waitingTime));
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
            CurrentTempo = ChangeTempo(_count);

            var time = (double)_playback.GetTime() / 1000f;
            var secondsPerBeat = CurrentTempo is TempoState.Normal or TempoState.PrevNormal ? 60f / info.bpm * 2 : 60f / info.bpm;

            var copy = new BeatInfo
            {
                Bpm = CurrentTempo == TempoState.Normal ? info.bpm / 2 : info.bpm,
                SecondsPerBeat = secondsPerBeat,
                BeatCount = info.beatCount,
                CurrentBeat = _count,
                NowTime = time,
                PrevBeatTime = time,
                NextBeatTime = time + secondsPerBeat,
                Playback = _playback,
            };

            return copy;
        }

        private void OnDisable()
        {
            BeatSyncDispatcher.Instance.Unregister(this);
        }

        private TempoState ChangeTempo(int count)
        {
            if (count > ChangeTempoBeat + BetweenBeats) return TempoState.Fast;
            if(count > ChangeTempoBeat) return TempoState.PrevFast;
            if (count > ChangeTempoBeat - BetweenBeats) return TempoState.None;
            if (count > PrepareBeat + BetweenBeats) return TempoState.Normal;
            if(count > PrepareBeat) return TempoState.PrevNormal;
            return TempoState.None;
        }
    }

    public struct BeatInfo
    {
        public float Bpm;
        public float SecondsPerBeat;
        public float BeatCount;
        public int CurrentBeat;
        public double NowTime;
        public double PrevBeatTime;
        public double NextBeatTime;
        public CriAtomExPlayback Playback;
    }

    public enum TempoState
    {
        PrevNormal,
        Normal,
        PrevFast,
        Fast,
        None
    }
}