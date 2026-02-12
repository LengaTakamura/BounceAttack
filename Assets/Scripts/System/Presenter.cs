using Player;
using R3;
using UI;
using UnityEngine;

namespace System
{
    public class Presenter : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private UiManager _uiManager;
        [SerializeField] private PlayerManager _playerManager;
        
        public int Health => _playerManager.CurrentHealth;
        
        private readonly CompositeDisposable _disposables = new();
        
        public ReadOnlyReactiveProperty<int> CurrentScore => _inputManager.CurrentScore;
        
        public Observable<int> ScoreChanged => _inputManager.ScoreChanged; 
        
        public Observable<BeatActionType> OnInputAction => _inputManager.OnInputAction;

        public Observable<Unit> OnAttack => _inputManager.OnAttack;
        
        public Observable<int> OnHit => _playerManager.OnHit;
        
        private void Start()
        {
            _uiManager.Init(this);
            OnAttack.Subscribe(_ => _playerManager.AttackEnemies()).AddTo(this);
        }


        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
