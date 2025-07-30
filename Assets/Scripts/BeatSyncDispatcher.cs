using System.Collections.Generic;
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
            listener.OnBeat();
        }
    }
}