using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UiManager : MonoBehaviour,IBeatSyncListener
    {
       [SerializeField] private List<UiBase> _uiObjects = new();
        private void Awake()
        {
            BeatSyncDispatcher.Instance.Register(this);
        }

        public void Init(Presenter presenter)
        {
            foreach (var uiObject in _uiObjects)
            {
                uiObject.Init(presenter);
            }
        }

        public void OnBeat(BeatInfo info)
        {
            foreach (var uiObject in _uiObjects)
            {
                uiObject.UIOnBeat(info);
            }
        }
        
    }

    public enum UiType
    {
        Score,InputAction,Player,Time,Enemy,None
    }
}
