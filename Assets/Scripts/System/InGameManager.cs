using UnityEngine;

namespace System
{
    public class InGameManager : MonoBehaviour
    {
        private BeatSystem _beatSystem;
        
        private void Start()
        {
            _beatSystem = BeatSyncDispatcher.Instance.Get<BeatSystem>();
        }
        
        
    }
}
