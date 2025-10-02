using System;
using UnityEngine;

namespace UI
{
    public abstract class UiBase : MonoBehaviour
    {
        [field: SerializeField] public UiType UiType { get; private set; } = UiType.None;
        public abstract void Init(GameEvents gameEvents);
        
        public abstract void UIOnBeat(BeatInfo info);
    }
}
