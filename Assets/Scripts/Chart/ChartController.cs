using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Chart
{
    public class ChartController : MonoBehaviour
    {
        private RectTransform _rectTransform;
        
        public Action OnDeath { get; set; }
        
        private CancellationTokenSource _cts;
        
        public void Init(RectTransform targetRectTransform,BeatInfo beatInfo)
        {
            _cts = new CancellationTokenSource();
            _rectTransform = GetComponent<RectTransform>();
            Move(targetRectTransform,beatInfo.SecondsPerBeat,_cts.Token).Forget();
        }

        private async UniTaskVoid Move(RectTransform targetRectTransform,float secondsPerBeat,CancellationToken token)
        {
            await _rectTransform.DOMove(targetRectTransform.anchoredPosition,secondsPerBeat * 4f).ToUniTask(cancellationToken: token);
            _cts?.Cancel();
            _cts?.Dispose();
            OnDeath?.Invoke();
            OnDeath = null;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
