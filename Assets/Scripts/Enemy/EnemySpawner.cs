using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour, IBeatSyncListener
{
    [SerializeField] private List<GameObject> _enemyList;
    [SerializeField] private List<Transform> _spawnPoints;
    private ObjectPool<EnemyBase> _pool;
    [SerializeField] private int _defaultSize;
    [SerializeField] private int _maxSize;
    private Action<BeatInfo> _onBeatAction;
    private BeatInfo _beatInfo;

    private Action _OnFixedUpdateAction;

    private Action _OnUpdateAction;

    [SerializeField] private PlayerManager _player;

    private void Awake()
    {
        BeatSyncDispatcher.Instance.Register(this);
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
        if ((int)_beatInfo.CurrentBeat % 3 == 0)
        {
            //if(_beatInfo.CurrentBeat < 10) return;
            DebugWave();
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

    private void DebugWave()
    {
        var random = Random.Range(0, 10);
        for (int i = 0; i < random; i++)
        {
            _pool.Get();
        }
    }
    private void OnDestroy()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }
}

public interface ITracking
{
    void SetTargetPosition(Func<Vector3> targetPositionProvider);
}
    