using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CriWare;

public class BeatSyncDispatcher : MonoBehaviour
{
    private readonly List<IBeatSyncListener> _listeners = new();

    public static BeatSyncDispatcher Instance;
    
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
        foreach (var listener in _listeners)
        {
            listener.OnBeat(ref info);
        }
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
}