using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace System
{
    public class BeatSystem : MonoBehaviour, IBeatSyncListener
    {
        [SerializeField] private SoundManager _soundManager;

        private CriAtomExPlayback _playback;

        private int _count;

        public float DefaultSecondsPerBeat => 60f / _defaultBpm;

        [SerializeField] private int _defaultBpm = 60;

        public TempoState CurrentTempo { get; private set; }

        [SerializeField]private int _changeTempoBeat = 40;
        [SerializeField] private int _prepareBeat = 5;
        [SerializeField]private int _betweenBeats = 4;
        [SerializeField] private float _waitingTime = 5f;

        public int BetweenBeats { get { return _betweenBeats; } }
        private bool _once;
        private bool _isWaiting;
        public bool IsWaiting {  get { return _isWaiting; } }
        private void Awake()
        {
            BeatSyncDispatcher.Instance.RegisterBeatSync(this);
            _betweenBeats = 4;
            _prepareBeat = 6;
            _isWaiting = true;
            _once = false;
            _count = -1;
        }

        private void Start()
        {
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            CurrentTempo = TempoState.None;
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
            _isWaiting = CurrentTempo != TempoState.Normal && CurrentTempo != TempoState.Fast;
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
            BeatSyncDispatcher.Instance.UnregisterBeatSync(this);
        }

        private TempoState ChangeTempo(int count)
        {
            if (count >= _changeTempoBeat + _betweenBeats - 1) return TempoState.Fast; // １テンポ目をとりやすくすためにー１
            if(count >= _changeTempoBeat) return TempoState.PrevFast;
            if (count >= _changeTempoBeat - _betweenBeats)
            {
                if (!_once)
                {
                    BeatSyncDispatcher.Instance.NotifyBreak();
                    Debug.Log("Break通知");
                    _once = true;
                    return TempoState.None;
                }
                return TempoState.None;
                
            }
            if (count >= _prepareBeat + _betweenBeats * 2 - 1 ) return TempoState.Normal; // 準備期間は長く １テンポ目をとりやすくすためにー１
            if(count >= _prepareBeat) return TempoState.PrevNormal;
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

    public interface IBreakListener
    {
        void OnBreak();
    }
}