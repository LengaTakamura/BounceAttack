using System.Collections.Generic;
using CriWare;
using UnityEngine;

namespace System
{
    public class BeatSyncDispatcher : MonoBehaviour
    {
        private readonly List<IBeatSyncListener> _listeners = new();

        public static BeatSyncDispatcher Instance;

        [SerializeField] private BeatSystem _beatSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CriAtomExBeatSync.OnCallback += ListenersOnBeat;
        }

        public void Register(IBeatSyncListener listener)
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

        public void Clear() => _listeners.Clear();

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

        public void Unregister(IBeatSyncListener listener)
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

        private void ListenersOnBeat(ref CriAtomExBeatSync.Info info)
        {
            var beatInfo = _beatSystem.UpdateInfo(info); //情報の更新
            if(_beatSystem.CurrentTempo == TempoState.Normal && beatInfo.CurrentBeat % 2 == 0) return;
            foreach (var listener in _listeners)
            {
                listener.OnBeat(beatInfo);　// 更新された情報を各要素に注入
            }
        }
    }

    public interface IBeatSyncListener
    {
        void OnBeat(BeatInfo info);
    }
}