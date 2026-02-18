using Chart;
using CriWare;
using Player;
using UI;
using Unity.Cinemachine;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;

namespace System
{
    public class InGameManager : MonoBehaviour
    {
        private InGameBeatSystem _beatSystem;
        private SoundManager _soundManager;
        private Presenter _presenter;
        private UiManager _uiManager;
        private PlayerManager _playerManager;
        private InputManager _inputManager;
        private UiView _uiView;
        private PlayerMove _move;

        [SerializeField] private EnemyLineSpawner _lineSpawner;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private ChartSpawner _chartSpawner;

        [SerializeField] private GameObject _inGameUiPrefab;

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private BeatSystemData _beatSystemData;
        [SerializeField] private SoundData _soundData;
        [SerializeField] private EnemyLineSpawnerData _enemyLineSpawnerData;
        [SerializeField] private EnemySpawnerData _enemySpawnerData;
        [SerializeField] private InputManagerData _inputManagerData;
        [SerializeField] private ChartSpawnerData _chartSpawnerData;
        [SerializeField] private ScoreData _scoreData;
        [SerializeField] private CriAtomSource _criAtomSource;
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private int _delay = 10;

        private CompositeDisposable _disposables = new();
        private void OnEnable()
        {
            Init();
        }

        /// <summary>
        /// �Q�[���J�n���̏�����
        /// </summary>
        private void Init()
        {
            _soundManager = new SoundManager();
            _beatSystem = new InGameBeatSystem(_soundManager);
            _uiManager = new UiManager();
            _inputManager = new InputManager(_beatSystem);
            var player = Instantiate(_playerPrefab, _playerSpawnPoint.position, Quaternion.identity);
            _move = player.GetComponent<PlayerMove>();
            _playerManager = player.GetComponent<PlayerManager>();
            _cinemachineCamera.Target.TrackingTarget = player.transform;
            _presenter = new Presenter(_playerManager, _uiManager, _inputManager,_scoreData);
            var ui = Instantiate(_inGameUiPrefab);
            _uiView = ui.GetComponent<UiView>();

            InitCommponents();
        }

        private void InitCommponents()
        {
            _soundManager.InGameInit(_criAtomSource, _soundData);
            BeatSyncDispatcher.Instance.InGameInit(_beatSystem);
            _beatSystem.InGameInit(_beatSystemData);
            _lineSpawner.InGameInit(_enemyLineSpawnerData);
            _inputManager.InGameInit(_inputManagerData);
            _playerManager.InGameInit(_inputManager);
            _presenter.InGameInit(_uiView);
            _chartSpawner.InGameInit(_beatSystem,_chartSpawnerData, _uiView.Canvas, _uiView.TargetImage);
            _enemySpawner.InGameInit(_playerManager, _enemySpawnerData);

            _playerManager.OnDeath.Subscribe(_ => Result()).AddTo(_disposables);
        }

        void Update()
        {
            _inputManager?.OnUpdate(_playerManager.IsDead);
            _move?.OnUpdate();
        }

        void FixedUpdate()
        {
            _move?.OnFixedUpdate();
        }

        private async void Result()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
            BeatSyncDispatcher.Instance.Clear();
            await GameSystem.Instance.ChangeScene("Novel");
        }

        void OnDisable()
        {
            _disposables?.Dispose();
            _beatSystem?.Dispose();
            _soundManager?.Dispose();
            _uiManager?.Dispose();
            _inputManager?.Dispose();
            _presenter?.Dispose();
        }
    }
}
