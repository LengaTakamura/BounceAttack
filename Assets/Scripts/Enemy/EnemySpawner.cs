using System;
using System.Collections.Generic;
using System.Threading;
using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour, IBeatSyncListener
{
    [SerializeField] private List<EnemyBase> _enemyList;
    [SerializeField] private List<Transform> _spawnPoints;
    private ObjectPool<EnemyBase> _pool;
    [SerializeField] private int _defaultSize;
    [SerializeField] private int _maxSize;
    private Action<BeatInfo> _onBeatAction;
    private BeatInfo _beatInfo;
    private BeatSystem _beatSystem;

    private void Start()
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
        enemyBase.Init(_beatInfo);
    }

    private EnemyBase InstantiatedEnemy()
    {
        var enemyIndex = Random.Range(0, _enemyList.Count);
        var obj = Instantiate(_enemyList[enemyIndex]);
        return obj;
    }
    
    public void OnBeat(BeatInfo beatInfo)
    {
        _beatInfo = beatInfo;
        _onBeatAction?.Invoke(_beatInfo);
        if ((int)_beatInfo.BeatCount % 3 == 0)
        {
            if(_beatInfo.CurrentBeat < 10) return;
            DebugWave();
        }
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
    