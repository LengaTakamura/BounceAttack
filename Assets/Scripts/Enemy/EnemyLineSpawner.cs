using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyLineSpawner : MonoBehaviour, IBeatSyncListener, IBreakListener
{
    //todo: 生成、破壊処理を外部に移す事でMonobehaviourから切り離すことができるかもしれないが、
    // Groundのサイズ等のSpawnerに依存する要素もあるため、現状はMonobehaviourのままにしている
    [SerializeField] private Renderer _groundPrefab;
    [SerializeField] private float _yOffset;
    private int _lineCount;
    private ObjectPool<EnemyBase> _pool;
    private int _defaultSize;
    private int _maxSize;
    private BeatInfo _beatInfo;
    private Action<BeatInfo> _onBeatAction;
    private List<EnemyBase> _enemyList;
    private List<SpawnPattern> _spawnPatternes;
    private int _spawnInterval;
    private List<EnemyBase> _activeEnemies = new List<EnemyBase>();
    private Action _OnFixedUpdateAction;
    private int _count;

    public void InGameInit(EnemyLineSpawnerData data)
    {
        _lineCount = data.LineCount;
        _defaultSize = data.DefaultSize;
        _maxSize = data.MaxSize;
        _enemyList = data.EnemyList;
        _spawnPatternes = data.SpawnPatterns;
        _spawnInterval = data.SpawnInterval;

        _count = 0;
        Init();

        BeatSyncDispatcher.Instance.RegisterBeatSync(this);
        BeatSyncDispatcher.Instance.RegisterBreak(this);

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
        if (enemyBase == null) return;

        if (enemyBase.gameObject != null)
        {
            Destroy(enemyBase.gameObject);
        }
    }

    private void ReleaseEnemy(EnemyBase enemyBase)
    {
        _onBeatAction -= enemyBase.EnemyOnBeat;
        _OnFixedUpdateAction -= enemyBase.OnFixedUpdate;
        enemyBase.gameObject.SetActive(false);
        _activeEnemies.Remove(enemyBase);
    }

    private void GetEnemy(EnemyBase enemyBase)
    {
        enemyBase.gameObject.SetActive(true);
        enemyBase.InitOnPool(() => _pool.Release(enemyBase));
        _onBeatAction += enemyBase.EnemyOnBeat;
        _OnFixedUpdateAction += enemyBase.OnFixedUpdate;
        _activeEnemies.Add(enemyBase);
        enemyBase.Init(_beatInfo);
    }

    private EnemyBase InstantiatedEnemy()
    {
        var obj = Instantiate(_enemyList[0]);
        var enemyBase = obj.GetComponent<EnemyBase>();
        return enemyBase;
    }

    public void OnBeat(BeatInfo beatInfo)
    {
        _beatInfo = beatInfo;
        _onBeatAction?.Invoke(_beatInfo);
        if ((int)_beatInfo.CurrentBeat % _spawnInterval == 0)
        {
            if (_count >= _spawnPatternes.Count)
            {
                _count = 0;
            }
            GetPool(_spawnPatternes[_count]);
            _count++;
        }
    }

    void FixedUpdate()
    {
        _OnFixedUpdateAction?.Invoke();
    }

    private void GetPool(SpawnPattern pattern)
    {
        switch (pattern)
        {
            case SpawnPattern.Horizontal:
                LeftSideSpawn();
                RightSideSpawn();
                break;
            case SpawnPattern.Vertical:
                UpSideSpawn();
                DownSideSpawn();
                break;
            case SpawnPattern.Both:
                LeftSideSpawn();
                RightSideSpawn();
                UpSideSpawn();
                DownSideSpawn();
                break;
            case SpawnPattern.UpSide:
                UpSideSpawn();
                break;
            case SpawnPattern.DownSide:
                DownSideSpawn();
                break;
            case SpawnPattern.LeftSide:
                LeftSideSpawn();
                break;
            case SpawnPattern.RightSide:
                RightSideSpawn();
                break;
        }
    }
    private void OnDisable()
    {
        _pool?.Dispose();
    }
    private void OnDestroy()
    {
        BeatSyncDispatcher.Instance.UnregisterBeatSync(this);
        BeatSyncDispatcher.Instance.UnregisterBreak(this);
    }

    private void LeftSideSpawn()
    {
        var minX = _groundPrefab.bounds.min.x;
        var virtical = _groundPrefab.bounds.size.z;
        var interval = virtical / (_lineCount - 1);
        for (int i = 0; i < _lineCount; i++)
        {
            var enemy = _pool.Get();
            enemy.transform.position = new Vector3(minX, _yOffset, _groundPrefab.bounds.min.z + interval * i);
            enemy.Direction = Vector3.right;
        }
    }

    private void RightSideSpawn()
    {
        var maxX = _groundPrefab.bounds.max.x;
        var virtical = _groundPrefab.bounds.size.z;
        var interval = virtical / (_lineCount - 1);
        for (int i = 0; i < _lineCount; i++)
        {
            var enemy = _pool.Get();
            enemy.transform.position = new Vector3(maxX, _yOffset, _groundPrefab.bounds.min.z + interval * i);
            enemy.Direction = Vector3.left;
        }
    }

    private void UpSideSpawn()
    {
        var maxZ = _groundPrefab.bounds.max.z;
        var width = _groundPrefab.bounds.size.x;
        var interval = width / (_lineCount - 1);
        for (int i = 0; i < _lineCount; i++)
        {
            var enemy = _pool.Get();
            enemy.transform.position = new Vector3(_groundPrefab.bounds.min.x + interval * i, _yOffset, maxZ);
            enemy.Direction = Vector3.back;
        }
    }

    private void DownSideSpawn()
    {
        var minZ = _groundPrefab.bounds.min.z;
        var width = _groundPrefab.bounds.size.x;
        var interval = width / (_lineCount - 1);
        for (int i = 0; i < _lineCount; i++)
        {
            var enemy = _pool.Get();
            enemy.transform.position = new Vector3(_groundPrefab.bounds.min.x + interval * i, _yOffset, minZ);
            enemy.Direction = Vector3.forward;
        }
    }

    public void OnBreak()
    {
        foreach (var enemy in _activeEnemies.ToArray())
        {
            enemy.Suicide();
        }
    }
}

public enum SpawnPattern
{
    Horizontal,
    Vertical,
    Both,
    UpSide,
    DownSide,
    LeftSide,
    RightSide
}

