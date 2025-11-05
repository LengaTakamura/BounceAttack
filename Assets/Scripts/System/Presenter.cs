using R3;
using UI;
using UnityEngine;

namespace System
{
    public class Presenter : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private UiManager _uiManager;
        
        private readonly CompositeDisposable _disposables = new();
        
        public ReadOnlyReactiveProperty<int> CurrentScore => _inputManager.CurrentScore;
        
        public Observable<int> ScoreChanged => _inputManager.ScoreChanged; 
        
        public Observable<BeatActionType> OnInputAction => _inputManager.OnInputAction;

        private void Start()
        {
            _uiManager.Init(this);
        }


        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
