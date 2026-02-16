using UnityEngine;
using UnityEngine.UI;

namespace Chart
{
    [CreateAssetMenu(fileName = "ChartSpawnerData", menuName = "Beat/ChartSpawnerData")]
    public class ChartSpawnerData : ScriptableObject
    {
        [Header("Note Settings")]
        [Tooltip("ノーツのプレハブ")]
        public ChartController ChartPrefab;

        [Tooltip("目標地点のイメージ")]
        public Image TargetImage;
        
        [Tooltip("ターゲットに到達するまでの猶予拍数")]
        public int BeatDelay = 4;

        [Header("Pool Settings")]
        public int DefaultSize = 10;
        public int MaxSize = 20;
    }
}