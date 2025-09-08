using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemyBase> _enemyList;
    [SerializeField] private List<Transform> _spawnPoints;
    private ObjectPool<EnemyBase> _pool;
    [SerializeField] private int _defaultSize;
    [SerializeField] private int _maxSize;
    private void Awake()
    {
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
        enemyBase.gameObject.SetActive(false);
    }

    private void GetEnemy(EnemyBase enemyBase)
    {
        var random = Random.Range(0,_spawnPoints.Count);
        enemyBase.transform.position = _spawnPoints[random].position;
        enemyBase.transform.rotation = _spawnPoints[random].rotation;
        enemyBase.gameObject.SetActive(true);
        enemyBase.InitOnPool(() => _pool.Release(enemyBase));
    }

    private EnemyBase InstantiatedEnemy()
    {
        var enemyIndex = Random.Range(0,_enemyList.Count);
        var obj = Instantiate(_enemyList[enemyIndex]);
        obj.gameObject.SetActive(false);
        return obj;
    }
}
