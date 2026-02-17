using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace System
{
    public class InGameBeatSystem : IBeatSyncListener, IDisposable
    {
        private SoundManager _soundManager;

        private CriAtomExPlayback _playback;

        private int _count;
        public TempoState CurrentTempo { get; private set; }

        private int _changeTempoBeat;
        private int _prepareBeat;
        private int _betweenBeats;
        private float _waitingTime;

        public int BetweenBeats { get { return _betweenBeats; } }
        private bool _once;
        private bool _isWaiting;
        public bool IsWaiting { get { return _isWaiting; } }

        public InGameBeatSystem(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        public void InGameInit(BeatSystemData beatSystemData)
        {
            _betweenBeats = beatSystemData.BetweenBeats;
            _prepareBeat = beatSystemData.prepareBeat;
            _changeTempoBeat = beatSystemData.ChangeTempoBeat;
            _waitingTime = beatSystemData.WaitingTime;

            _isWaiting = true;
            _once = false;
            _count = -1;

            BeatSyncDispatcher.Instance.RegisterBeatSync(this);
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            CurrentTempo = TempoState.None;
            await UniTask.Delay(TimeSpan.FromSeconds(_waitingTime));

            _playback = _soundManager.PlayBgm();
            if (!_playback.GetBeatSyncInfo(out _))
            {
                Debug.LogWarning("BeatSync info could not be acquired from playback. Verify the selected cue has BeatSync settings.");
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

        private TempoState ChangeTempo(int count)
        {
            if (count >= _changeTempoBeat + _betweenBeats - 1) return TempoState.Fast;
            if (count >= _changeTempoBeat) return TempoState.PrevFast;
            if (count >= _changeTempoBeat - _betweenBeats)
            {
                if (!_once)
                {
                    BeatSyncDispatcher.Instance.NotifyBreak();
                    Debug.Log("Break triggered");
                    _once = true;
                    return TempoState.None;
                }
                return TempoState.None;
            }
            if (count >= _prepareBeat + _betweenBeats * 2 - 1) return TempoState.Normal;
            if (count >= _prepareBeat) return TempoState.PrevNormal;
            return TempoState.None;
        }

        public void Dispose()
        {
            BeatSyncDispatcher.Instance.UnregisterBeatSync(this);
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
