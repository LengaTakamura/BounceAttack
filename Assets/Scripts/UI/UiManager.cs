using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UiManager : MonoBehaviour,IBeatSyncListener
    {
        
       [SerializeField] private List<UiBase> _uiObjects = new();
       
        private GameEvents _gameEvents;
        private void Start()
        {
            BeatSyncDispatcher.Instance.Register(this);
            _gameEvents = GameEvents.Instance;
            foreach (var uiObject in _uiObjects)
            {
                UiObjectsInit(uiObject);
            }
        }

        public void OnBeat(BeatInfo info)
        {
            foreach (var uiObject in _uiObjects)
            {
                uiObject.UIOnBeat(info);
            }
        }

        private void UiObjectsInit(UiBase ui)
        {
           ui.Init(_gameEvents);
        }
    }

    public enum UiType
    {
        Score,InputAction,Time,Enemy,None
    }
}
