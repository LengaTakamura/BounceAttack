using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerData", menuName = "Beat/EnemySpawnerData")]
public class EnemySpawnerData : ScriptableObject
{
    [Header("Spawn Settings")]
    [Tooltip("何拍ごとにスポーン判定を行うか")]
    public int SpawnInterval = 3;

    [Tooltip("一度にスポーンする最小数")]
    public int MinSpawnCount = 1;

    [Tooltip("一度にスポーンする最大数")]
    public int MaxSpawnCount = 10;

    [Header("Object Pool Settings")]
    public int DefaultSize = 10;
    public int MaxSize = 50;

    [Header("Prefabs")]
    [Tooltip("出現候補の敵プレハブリスト")]
    public List<GameObject> EnemyPrefabs;
}