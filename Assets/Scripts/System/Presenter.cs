using Player;
using R3;
using UI;

namespace System
{
    public class Presenter : IDisposable
    {
        private InputManager _inputManager;
        private UiManager _uiManager;
        private PlayerManager _playerManager;
        
        public int Health => _playerManager.CurrentHealth;
        
        private readonly CompositeDisposable _disposables = new();
        
        public ReadOnlyReactiveProperty<int> CurrentScore => _inputManager.CurrentScore;
        
        public Observable<int> ScoreChanged => _inputManager.ScoreChanged; 
        
        public Observable<BeatActionType> OnInputAction => _inputManager.OnInputAction;

        public Observable<Unit> OnAttack => _inputManager.OnAttack;
        
        public Observable<int> OnHit => _playerManager.OnHit;

        public Presenter(PlayerManager playerManager, UiManager uiManager, InputManager inputManager)
        {
            _playerManager = playerManager;
            _uiManager = uiManager;
            _inputManager = inputManager;
        }

        
        public void InGameInit(UiView uiView)
        {
            _uiManager.InGameInitByPresenter(this ,uiView);
            //InputManagerの初期化はInGameManagerで行う
            //PresenterはInputManagerを参照するため、InputManagerの初期化後にPresenterの初期化を行う必要がある
            OnAttack.Subscribe(_ => _playerManager.AttackEnemies()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
