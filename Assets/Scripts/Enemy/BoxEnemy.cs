using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class BoxEnemy : EnemyBase
    {
        private int _count;
        
        private Sequence _seq;

        [SerializeField] private int _delay = 10;
        
        private CancellationTokenSource _cts;
        public override void Init(BeatInfo beatInfo)
        {
            Move(beatInfo.SecondsPerBeat).Forget();
            _count = -1;
        }

        private async UniTaskVoid Move(float secondsPerBeat)
        {
            var randomX = Random.Range(0f, 10f);
            var randomY = Random.Range(0f, 10f);
            var randomZ = Random.Range(0f, 10f);
            _cts = new CancellationTokenSource();
            await transform.DOLocalMove(new Vector3(randomX, randomY, randomZ), secondsPerBeat * _delay).SetEase(Ease.Linear).ToUniTask(cancellationToken: _cts.Token);
            Suicide();
        }
        public override void EnemyOnBeat(BeatInfo info)
        {
            base.EnemyOnBeat(info);
            _count++;
            if (_count % 2 == 0)
            {
                _seq?.Kill();
                _seq = DOTween.Sequence()
                    .Append(transform.DOScale(new Vector3(3f, 3f, 3f), info.SecondsPerBeat))
                    .Append(transform.DOScale(new Vector3(1f, 1f, 1f), info.SecondsPerBeat));
            }
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out PlayerManager playerHealth)) return;
            OnAttack(playerHealth);
            Suicide();
        }
    }
}
