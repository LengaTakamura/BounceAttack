using System;
using System.Collections.Generic;
using UnityEngine;
using CriWare;

public class BeatSyncDispatcher : MonoBehaviour
{
    private readonly List<IBeatSyncListener> _listeners = new();

    public static BeatSyncDispatcher Instance;
    
    private BeatSystem _beatSystem;
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

    private void Start()
    {
        _beatSystem = Get<BeatSystem>();
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
        foreach (var listener in _listeners)
        {
            var beatInfo = _beatSystem.UpdateInfo(info);
            listener.OnBeat(beatInfo);
        }
    }
    
 
    
}
public interface IBeatSyncListener
{
    void OnBeat(BeatInfo info);
    
}