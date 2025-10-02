using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace UI
{
    public class UiManager : MonoBehaviour,IBeatSyncListener
    {
        
       [SerializeField] private List<UiBase> _uiObjects = new();
       
        private GameEvents _gameEvents;
        private void Awake()
        {
            BeatSyncDispatcher.Instance.Register(this);
            _gameEvents = GameEvents.Instance;
            foreach (var uiObject in _uiObjects)
            {
                SwitchSubscribe(uiObject);
            }
        }

        public void OnBeat(BeatInfo info)
        {
            foreach (var uiObject in _uiObjects)
            {
                uiObject.UIOnBeat(info);
            }
        }

        private void SwitchSubscribe(UiBase ui)
        {
            switch (ui.UiType)
            {
                case UiType.Score:
                {
                    ui.Init(_gameEvents);
                    break;
                }
                
            }
            
        }
    }

    public enum UiType
    {
        Score,Time,Enemy,None
    }
}
