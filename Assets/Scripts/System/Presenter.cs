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
        private ScoreData _scoreData;

        public int Health => _playerManager.CurrentHealth;

        private readonly CompositeDisposable _disposables = new();

        public ReadOnlyReactiveProperty<int> CurrentScore => _inputManager.CurrentScore;
        public Observable<BeatActionType> OnInputAction => _inputManager.OnInputAction;
        public Observable<int> OnDeathWithScore => _playerManager.OnDeath.Select(_ => _inputManager.CurrentScore.CurrentValue);
        public Observable<Unit> OnAttack => _inputManager.OnAttack;

        public Observable<int> OnHit => _playerManager.OnHit;

        public Presenter(PlayerManager playerManager, UiManager uiManager, InputManager inputManager, ScoreData data)
        {
            _playerManager = playerManager;
            _uiManager = uiManager;
            _inputManager = inputManager;
            _scoreData = data;
        }


        public void InGameInit(UiView uiView)
        {
            _uiManager.InGameInitByPresenter(this, uiView);
            //InputManagerの初期化はInGameManagerで行う
            //PresenterはInputManagerを参照するため、InputManagerの初期化後にPresenterの初期化を行う必要がある
            OnAttack.Subscribe(_ => _playerManager.AttackEnemies()).AddTo(_disposables);
            _playerManager.ScoreChanged.Subscribe(score => _inputManager.AddScore(score)).AddTo(_disposables);
            CurrentScore.Subscribe(score => SetScore(score)).AddTo(_disposables);
        }

        private void SetScore(int score)
        {
            if (_scoreData == null) return;
            _scoreData.Score = score;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
