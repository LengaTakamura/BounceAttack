using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Chart
{
   public class ChartSpawner : MonoBehaviour,IBeatSyncListener
   {
      [SerializeField] private ChartController _chart;
      
      [SerializeField] private Canvas _canvas;

      [SerializeField] private Image _targetImage;
      
      private ObjectPool<ChartController> _chartImagePool;
      [SerializeField] private int _defaultSize;
      [SerializeField] private int _maxSize;
      private Vector2 _initialPosition;
      private BeatInfo _beatInfo;
      private BeatSystem _beatSystem;
      [SerializeField] private int _beatDelay = 4;
      private void Awake()
      {
         BeatSyncDispatcher.Instance.Register(this);
      }

      private void Start()
      {
         Init();
      }

      private void Init()
      {
         _chartImagePool = new ObjectPool<ChartController>(
            createFunc: InstantiateChart,
            actionOnGet: GetChart,
            actionOnRelease: ReleaseChart,
            actionOnDestroy: DestroyChart,
            collectionCheck: true,
            defaultCapacity: _defaultSize,
            maxSize: _maxSize
         );
         _initialPosition = _chart.GetComponent<RectTransform>().anchoredPosition;
         _beatSystem = BeatSyncDispatcher.Instance.Get<BeatSystem>();
      }

      private ChartController InstantiateChart()
      {
         var chart = Instantiate(_chart,_canvas.transform);
         return chart;
      }

      private void GetChart(ChartController chart)
      {
         chart.GetComponent<RectTransform>().anchoredPosition = _initialPosition;
         chart.gameObject.SetActive(true);
         chart.Init(_targetImage.rectTransform,_beatInfo,_beatDelay);
         chart.OnDeath += () => _chartImagePool.Release(chart);
      }

      private void ReleaseChart(ChartController chart)
      {
         chart.gameObject.SetActive(false);
      }

      private void DestroyChart(ChartController chart)
      {
         Destroy(chart.gameObject);
      }

      public void OnBeat(BeatInfo info)
      {
         _beatInfo = info;
         GetPool();
      }

      private void GetPool()
      {
         _chartImagePool.Get();
      }

      private void OnDestroy()
      {
         BeatSyncDispatcher.Instance.Unregister(this);
      }
   }
}
