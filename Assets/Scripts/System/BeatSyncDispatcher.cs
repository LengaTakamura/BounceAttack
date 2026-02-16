using System.Collections.Generic;
using CriWare;
using UnityEngine;

namespace System
{
    public class BeatSyncDispatcher : MonoBehaviour
    {
        private readonly List<IBeatSyncListener> _listeners = new();

        public static BeatSyncDispatcher Instance;

        private InGameBeatSystem _beatSystem;
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
            _beatSystem = beatSystem;
            Init();
            _isInGame = true;
        }
        public void Init()
        {
            CriAtomExBeatSync.OnCallback += ListenersOnBeat;
            _isInGame = false;
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
                var beatInfo = _beatSystem.UpdateInfo(info); //情報の更新
                if (_beatSystem.CurrentTempo == TempoState.Normal && beatInfo.CurrentBeat % 2 == 1) return;
                if (_beatSystem.CurrentTempo == TempoState.PrevNormal && beatInfo.CurrentBeat % 2 == 1) return;
                if (_beatSystem.CurrentTempo == TempoState.None) return;
                foreach (var listener in _listeners)
                {
                    listener.OnBeat(beatInfo); // 更新された情報を各要素に注入
                }
            }
            else
            {
                // アウトゲーム版BeatSystemはまだないので、BeatSyncDispatcherを経由してBeatInfoを渡すことができない。
                // アウトゲーム版BeatSystemができたら、同様にBeatInfoを更新してから各要素に注入する形にする予定。
                 Debug.LogWarning("アウトゲーム版BeatSystemはまだ実装されていません");
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