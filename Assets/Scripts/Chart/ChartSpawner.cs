using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Chart
{
   public class ChartSpawner : MonoBehaviour, IBeatSyncListener, IBreakListener
   {
      private ChartController _chart;

      private Canvas _canvas;

      private Image _targetImage;

      private ObjectPool<ChartController> _chartImagePool;

      private int _defaultSize;
      private int _maxSize;
      private Vector2 _initialPosition;
      private BeatInfo _beatInfo;
      private InGameBeatSystem _beatSystem;
      private int _beatDelay = 4;
      private List<ChartController> _activeCharts = new List<ChartController>();
      public void InGameInit(ChartSpawnerData data, Canvas canvas,Image targetImage)
      {
         _chart = data.ChartPrefab;
         _defaultSize = data.DefaultSize;
         _maxSize = data.MaxSize;
         _beatDelay = data.BeatDelay;
         
         _targetImage = targetImage;
         _canvas = canvas;

         BeatSyncDispatcher.Instance.RegisterBeatSync(this);
         BeatSyncDispatcher.Instance.RegisterBreak(this);

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
         _beatSystem = BeatSyncDispatcher.Instance.Get<InGameBeatSystem>();
         _beatDelay = _beatSystem.BetweenBeats;
      }

      private ChartController InstantiateChart()
      {
         var chart = Instantiate(_chart, _canvas.transform);
         return chart;
      }

      private void GetChart(ChartController chart)
      {
         chart.GetComponent<RectTransform>().anchoredPosition = _initialPosition;
         chart.gameObject.SetActive(true);
         chart.Init(_targetImage.rectTransform, _beatInfo, _beatDelay);
         chart.OnDeath += () => _chartImagePool.Release(chart);
         _activeCharts.Add(chart);
      }

      private void ReleaseChart(ChartController chart)
      {
         _activeCharts.Remove(chart);
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

      private void HideChart()
      {
         foreach (var chartController in _activeCharts.ToArray())
         {
            chartController.gameObject.SetActive(false);
         }
         _activeCharts.Clear();
      }

      private void OnDestroy()
      {
         BeatSyncDispatcher.Instance.UnregisterBreak(this);
         BeatSyncDispatcher.Instance.UnregisterBeatSync(this);
      }

      public void OnBreak()
      {
         HideChart();
      }
   }
}
