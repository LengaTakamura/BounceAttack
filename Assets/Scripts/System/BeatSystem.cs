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

        private float _prevTime;
        
        public float DefaultSecondsPerBeat => 60f / _defaultBpm;
        
        [SerializeField] private int _defaultBpm = 60;

        public TempoState CurrentTempo { get;private set;}
        
        [SerializeField] private int _changeTempoBeat = 50;
        
        private void Start()
        {
            BeatSyncDispatcher.Instance.Register(this);
            Init().Forget();
            _count = -1;
        }

        private async UniTaskVoid Init()
        {
            CurrentTempo = TempoState.None;
            await UniTask.Delay(TimeSpan.FromSeconds(5f));
            _playback = _soundManager.PlayBgm();
            CurrentTempo = TempoState.Normal;
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
            
            if (_count == _changeTempoBeat)
            {
                ChangeTempo();
            }

            var time = (ulong)_playback.GetTime() / (ulong)1000f;
            var secondsPerBeat = CurrentTempo == TempoState.Normal ? 60f / info.bpm * 2 : 60f / info.bpm;
            
            var copy = new BeatInfo
            {
                Bpm = CurrentTempo == TempoState.Normal ? info.bpm / 2 : info.bpm,
                SecondsPerBeat = secondsPerBeat,
                BeatCount = info.beatCount,
                CurrentBeat = _count,
                NowTime = time,
                PrevBeatTime = time,
                NextBeatTime = time + (ulong)secondsPerBeat,
                Playback = _playback,
            };
            
            return copy;
        }
    
        private void OnDisable()
        {
            BeatSyncDispatcher.Instance.Unregister(this);
        }

        private void ChangeTempo()
        {
            CurrentTempo = TempoState.Fast;
        }
    }
    public struct BeatInfo
    {
        public float Bpm;
        public float SecondsPerBeat;
        public float BeatCount;
        public int CurrentBeat;
        public ulong NowTime;
        public ulong PrevBeatTime;
        public ulong NextBeatTime;
        public CriAtomExPlayback Playback;
    }

    public enum TempoState
    {
        Normal,Fast,None
    }
}