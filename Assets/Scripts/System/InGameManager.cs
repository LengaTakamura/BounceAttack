using Chart;
using CriWare;
using Player;
using UI;
using UnityEngine;

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
        [SerializeField] private CriAtomSource _criAtomSource;
        [SerializeField] private Transform _playerSpawnPoint;

        private void OnEnable()
        {
            Init();
        }

        /// <summary>
        /// 最初のゲーム開始時の初期化処理
        /// </summary>
        private void Init()
        {
            _soundManager = new SoundManager();
            _beatSystem = new InGameBeatSystem(_soundManager);
            _uiManager = new UiManager();
            _inputManager = new InputManager(_beatSystem);
            var player = Instantiate(_playerPrefab, _playerSpawnPoint.position, Quaternion.identity);
            _playerManager = player.GetComponent<PlayerManager>();
            _presenter = new Presenter(_playerManager, _uiManager, _inputManager);
            var ui = Instantiate(_inGameUiPrefab);
            _uiView = ui.GetComponent<UiView>();

            InitCommponents();
        }

        private void InitCommponents()
        {
            _beatSystem.InGameInit(_beatSystemData);
            _soundManager.InGameInit(_criAtomSource, _soundData);
            _lineSpawner.InGameInit(_enemyLineSpawnerData);
            _enemySpawner.InGameInit(_enemySpawnerData);
            _inputManager.InGameInit(_inputManagerData);
            _presenter.InGameInit(_uiView);
            _chartSpawner.InGameInit(_chartSpawnerData);
            _playerManager.InGameInit();
        }

        void Update()
        {
            _inputManager?.OnUpdate();
        }

        void OnDisable()
        {
            _beatSystem?.Dispose();
            _soundManager?.Dispose();
            _uiManager?.Dispose();
            _inputManager?.Dispose();
            _presenter?.Dispose();
        }


    }
}
