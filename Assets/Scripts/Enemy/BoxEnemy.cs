using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Enemy
{
    public class BoxEnemy : EnemyBase, ITracking
    {
        private int _count;
        
        private Sequence _seq;

        [SerializeField] private int _delay = 10;
        
        private CancellationTokenSource _cts;

        private Func<Vector3> _setTargetPosition;

        private Vector3 _targetPos;

        [SerializeField] private float _moveSpeed = 5f;

        public override void Init(BeatInfo beatInfo)
        {
            StartMoving(beatInfo.SecondsPerBeat).Forget();
            _count = -1;
        }

        private async UniTaskVoid StartMoving(float secondsPerBeat)
        {
            _cts = new CancellationTokenSource();
            await UniTask.Delay(TimeSpan.FromSeconds(secondsPerBeat * _delay), cancellationToken: _cts.Token);
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

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            _targetPos = _setTargetPosition?.Invoke() ?? transform.position;
            Move();
        }

        private void Move()
        {
            transform.position += Direction * _moveSpeed * Time.deltaTime;
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
            if (!other.gameObject.TryGetComponent(out PlayerManager playerManager)) return;
            OnAttack(playerManager);
            Suicide();
        }

        public void SetTargetPosition(Func<Vector3> targetPositionProvider)
        {
            _setTargetPosition = targetPositionProvider;
        }
    }
}
