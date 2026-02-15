using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyLineSpawner : MonoBehaviour, IBeatSyncListener, IBreakListener
{
    [SerializeField] private Renderer _groundPrefab;
    [SerializeField] private int _lineCount;
    private ObjectPool<EnemyBase> _pool;
    [SerializeField] private int _defaultSize;
    [SerializeField] private int _maxSize;
    private BeatInfo _beatInfo;
    private Action<BeatInfo> _onBeatAction;
    [SerializeField] private List<EnemyBase> _enemyList;
    [SerializeField] private List<SpawnPattern> _spawnPatternes;
    [SerializeField] private int _spawnInterval;
    private List<EnemyBase> _activeEnemies = new List<EnemyBase>();
    private Action _OnFixedUpdateAction;
    private int _count;

    private void Start()
    {
        BeatSyncDispatcher.Instance.RegisterBeatSync(this);
        BeatSyncDispatcher.Instance.RegisterBreak(this);
        Init();
        _count = 0;
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
            enemy.transform.position = new Vector3(minX, 3, _groundPrefab.bounds.min.z + interval * i);
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
            enemy.transform.position = new Vector3(maxX, 3, _groundPrefab.bounds.min.z + interval * i);
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
            enemy.transform.position = new Vector3(_groundPrefab.bounds.min.x + interval * i, 3, maxZ);
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
            enemy.transform.position = new Vector3(_groundPrefab.bounds.min.x + interval * i, 3, minZ);
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

