using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyLineSpawnerData", menuName = "Beat/EnemyLineSpawnerData")]
public class EnemyLineSpawnerData : ScriptableObject
{
    [Header("Spawner Settings")]
    [Tooltip("敵のラインの数")]
    public int LineCount;

    [Tooltip("スポーン間隔（伯単位）")]
    public int SpawnInterval;

    [Header("Object Pool Settings")]
    public int DefaultSize = 100;
    public int MaxSize = 200;

    [Header("Prefabs & Patterns")]
    [Tooltip("生成する敵のプレハブリスト")]
    public List<EnemyBase> EnemyList;

    [Tooltip("出現パターンのシーケンス")]
    public List<SpawnPattern> SpawnPatterns;
}
