using System;
using System.Collections.Generic;
using System.Threading;
using CriWare;
using Cysharp.Threading.Tasks;
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
        DebugWave().Forget();
    }

    private async UniTaskVoid DebugWave()
    {
        var cts = new CancellationTokenSource();
        int count = 0;
        while (count < 50)
        {
            count++;
            var random = UnityEngine.Random.Range(0, 10);
            for (int i = 0; i < random; i++)
            {
                _pool.Get();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cts.Token);
        }
        cts.Cancel();
        cts.Dispose();
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

        for (int i = 0; i < _defaultSize; i++)
        {
            var e =  _pool.Get();
            ReleaseEnemy(e);
        }
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
        Debug.Log(enemyBase);
        enemyBase.gameObject.SetActive(true);
        var random = Random.Range(0,_spawnPoints.Count);
        enemyBase.transform.position = _spawnPoints[random].position;
        enemyBase.transform.rotation = _spawnPoints[random].rotation;
        enemyBase.InitOnPool(() => _pool.Release(enemyBase));
        _onBeatAction += enemyBase.EnemyOnBeat;
    }

    private EnemyBase InstantiatedEnemy()
    {
        var enemyIndex = Random.Range(0,_enemyList.Count);
        var obj = Instantiate(_enemyList[enemyIndex]);
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
