using System.Collections.Generic;
using CriWare;
using UnityEngine;

namespace System
{
    public class BeatSyncDispatcher : MonoBehaviour
    {
        private readonly List<IBeatSyncListener> _listeners = new();

        public static BeatSyncDispatcher Instance;

        private InGameBeatSystem _ingameBeatSystem;
        private NovelBeatSystem _novelBeatSystem;
        private readonly List<IBreakListener> _breakables = new();
        private bool _isInGame;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void InGameInit(InGameBeatSystem beatSystem)
        {
            _ingameBeatSystem = beatSystem;
            CriAtomExBeatSync.OnCallback += ListenersOnBeat;
            _isInGame = true;
        }
        public void NovelInit(NovelBeatSystem novelBeatSystem)
        {
            _novelBeatSystem = novelBeatSystem;
            CriAtomExBeatSync.OnCallback += ListenersOnBeat;
            _isInGame = false;
        }

        public void Clear()
        {
            ClearBeatSync();
            CriAtomExBeatSync.OnCallback -= ListenersOnBeat;
        }
        public void RegisterBeatSync(IBeatSyncListener listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
            else
            {
                Debug.LogWarning("listener already registered");
            }
        }

        public void RegisterBreak(IBreakListener breakable)
        {
            if (!_breakables.Contains(breakable))
            {
                _breakables.Add(breakable);
            }
            else
            {
                Debug.LogWarning("breakable already registered");
            }
        }
        public void ClearBeatSync() => _listeners.Clear();

        public T Get<T>() where T : class, IBeatSyncListener
        {
            foreach (var listener in _listeners)
            {
                if (listener is T tListener)
                {
                    return tListener;
                    // クラスでありIBeatSyncListenerを継承しているTを探してTを返す
                }
            }

            return null;
        }

        public void UnregisterBeatSync(IBeatSyncListener listener, IBreakListener breakable = null)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
            else
            {
                Debug.LogWarning("listener not registered");
            }
        }

        public void UnregisterBreak(IBreakListener breakable)
        {
            if (_breakables.Contains(breakable))
            {
                _breakables.Remove(breakable);
            }
            else
            {
                Debug.LogWarning("breakable not registered");
            }
        }

        private void ListenersOnBeat(ref CriAtomExBeatSync.Info info)
        {
            if (_isInGame)
            {
                var beatInfo = _ingameBeatSystem.UpdateInfo(info); //情報の更新
                if (_ingameBeatSystem.CurrentTempo == TempoState.Normal && beatInfo.CurrentBeat % 2 == 1) return;
                if (_ingameBeatSystem.CurrentTempo == TempoState.PrevNormal && beatInfo.CurrentBeat % 2 == 1) return;
                if (_ingameBeatSystem.CurrentTempo == TempoState.None) return;
                foreach (var listener in _listeners)
                {
                    listener.OnBeat(beatInfo); // 更新された情報を各要素に注入
                }
            }
            else
            {
                var beatInfo = _novelBeatSystem.UpdateInfo(info);
                foreach(var listener in _listeners)
                {
                    listener.OnBeat(beatInfo);
                }
            }
        }

        public void NotifyBreak()
        {
            foreach (var breakable in _breakables)
            {
                breakable.OnBreak();
            }
        }
    }

    public interface IBeatSyncListener
    {
        void OnBeat(BeatInfo info);
    }
}