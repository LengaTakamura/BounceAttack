using System;
using System.Collections.Generic;
using CriWare;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour,IBeatSyncListener
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
        _beatSystem = BeatSyncDispatcher.Instance.Get<BeatSystem>();
        _beatSystem.OnBeatAction += UpdateSpawnerInfo;
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
        _onBeatAction += enemyBase.UpdateInfo;
        enemyBase.gameObject.SetActive(false);
    }

    private void GetEnemy(EnemyBase enemyBase)
    {
        var random = Random.Range(0,_spawnPoints.Count);
        enemyBase.transform.position = _spawnPoints[random].position;
        enemyBase.transform.rotation = _spawnPoints[random].rotation;
        enemyBase.gameObject.SetActive(true);
        enemyBase.InitOnPool(() => _pool.Release(enemyBase));
        _onBeatAction += enemyBase.UpdateInfo;
    }

    private EnemyBase InstantiatedEnemy()
    {
        var enemyIndex = Random.Range(0,_enemyList.Count);
        var obj = Instantiate(_enemyList[enemyIndex]);
        obj.gameObject.SetActive(false);
        return obj;
    }

    private void UpdateSpawnerInfo(BeatInfo beatInfo)
    {
        _beatInfo = beatInfo;
    }

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        _onBeatAction?.Invoke(_beatInfo);
    }

    private void OnDestroy()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }
}
