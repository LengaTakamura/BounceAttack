using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour, IBeatSyncListener,IBreakListener
{
    //todo: 敵のスポーン位置を属性で指定できるようにしたり、生成・破壊時の処理を別に移すことでMonobehaviourから切り離すことができるかもしれないが、
    // 現状はシンプルにするためにMonobehaviourのままにしている

    [SerializeField] private List<Transform> _spawnPoints;
    private List<GameObject> _enemyList;
    private ObjectPool<EnemyBase> _pool;
    private int _defaultSize;
    private int _maxSize;
    private int _spawnInterval;
    private int _minSpawnCount;
    private int _maxSpawnCount;
    private Action<BeatInfo> _onBeatAction;
    private BeatInfo _beatInfo;
    private Action _OnFixedUpdateAction;
    private Action _OnUpdateAction;
    private List<EnemyBase> _activeEnemies = new List<EnemyBase>();
    private PlayerManager _player;

    public void InGameInit(PlayerManager playerManager , EnemySpawnerData data)
    {
        _enemyList = data.EnemyPrefabs;
        _defaultSize = data.DefaultSize;
        _maxSize = data.MaxSize;
        _spawnInterval = data.SpawnInterval;
        _minSpawnCount = data.MinSpawnCount;
        _maxSpawnCount = data.MaxSpawnCount;

        _player = playerManager;

        BeatSyncDispatcher.Instance.RegisterBeatSync(this);
        BeatSyncDispatcher.Instance.RegisterBreak(this);
        
        Init();
    }

    private void Init()
    {
        _pool = new ObjectPool<EnemyBase>(
            createFunc: InstantiatedEnemy,
            actionOnGet: GetEnemy,
            actionOnRelease: ReleaseEnemy,
            actionOnDestroy: DestroyEnemy,
            collectionCheck: true,
            defaultCapacity: _defaultSize,
            maxSize: _maxSize
        );
    }

    private void DestroyEnemy(EnemyBase enemyBase)
    {
        Destroy(enemyBase.gameObject);
    }

    private void ReleaseEnemy(EnemyBase enemyBase)
    {
        _onBeatAction -= enemyBase.EnemyOnBeat;
        _OnFixedUpdateAction -= enemyBase.OnFixedUpdate;
        _OnUpdateAction -= enemyBase.OnUpdate;
        enemyBase.gameObject.SetActive(false);
        _activeEnemies.Remove(enemyBase);
    }

    private void GetEnemy(EnemyBase enemyBase)
    {
        enemyBase.gameObject.SetActive(true);
        var random = Random.Range(0, _spawnPoints.Count);
        enemyBase.transform.position = _spawnPoints[random].position;
        enemyBase.transform.rotation = _spawnPoints[random].rotation;
        enemyBase.InitOnPool(() => _pool.Release(enemyBase));
        _onBeatAction += enemyBase.EnemyOnBeat;
        _OnFixedUpdateAction += enemyBase.OnFixedUpdate;
        _OnUpdateAction += enemyBase.OnUpdate;
        _activeEnemies.Add(enemyBase);
        if(enemyBase is ITracking tracking)
        {
            tracking.SetTargetPosition(() => _player.transform.position);
        }
        enemyBase.Init(_beatInfo);
    }

    private EnemyBase InstantiatedEnemy()
    {
        var enemyIndex = Random.Range(0, _enemyList.Count);
        var obj = Instantiate(_enemyList[enemyIndex]);
        var enemyBase = obj.GetComponent<EnemyBase>();
        return enemyBase;
    }
    
    public void OnBeat(BeatInfo beatInfo)
    {
        _beatInfo = beatInfo;
        _onBeatAction?.Invoke(_beatInfo);
        if ((int)_beatInfo.CurrentBeat % _spawnInterval == 0)
        {
            Wave();
        }
    }

    void FixedUpdate()
    {
        _OnFixedUpdateAction?.Invoke();
    }

    void Update()
    {
        _OnUpdateAction?.Invoke();
    }

    private void Wave()
    {
        var random = Random.Range(_minSpawnCount, _maxSpawnCount + 1);
        for (int i = 0; i < random; i++)
        {
            _pool.Get();
        }
    }
    private void OnDestroy()
    {
        BeatSyncDispatcher.Instance.UnregisterBeatSync(this);
        BeatSyncDispatcher.Instance.UnregisterBreak(this);
    }

    public void OnBreak()
    {
        foreach (var enemy in _activeEnemies.ToArray())
        {
            enemy.Suicide();
        }
    }
}

public interface ITracking
{
    void SetTargetPosition(Func<Vector3> targetPositionProvider);
}
    