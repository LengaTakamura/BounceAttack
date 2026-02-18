using System;
using R3;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private int _currentHealth;

        public int CurrentHealth => _currentHealth;

        [SerializeField] private int _maxHealth;

        [SerializeField] private float _attackRadius;

        [SerializeField] private LayerMask _enemyLayerMask;

        private readonly Subject<int> _onHit = new();
        public Observable<int> OnHit => _onHit;
        private readonly Subject<Unit> _onDeath = new();
        public Observable<Unit> OnDeath => _onDeath;
        private readonly Subject<int> _scoreChanged = new();
        public Observable<int> ScoreChanged => _scoreChanged;
        private PlayerMove _move;
        private bool _isDead;
        public bool IsDead => _isDead;
        public void InGameInit(InputManager inputManager)
        {
            _currentHealth = _maxHealth;
            _move = GetComponent<PlayerMove>();
            _move.Init(inputManager);
            _isDead = false;
        }

        public void TakeDamage(int damage)
        {
            if(IsDead) return;
            if (_move.IsBlinking) return;
            _currentHealth -= damage;
            _onHit?.OnNext(damage);
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _isDead = true;
                _onDeath?.OnNext(default);
            }
        }

        public void AttackEnemies()
        {
            if(IsDead) return;
            var array = new Collider[100];
            var count = Physics.OverlapSphereNonAlloc(transform.position, _attackRadius, array, _enemyLayerMask);
            for (int i = 0; i < count; i++)
            {
                if (!array[i].gameObject.TryGetComponent(out EnemyBase enemyBase)) continue;
                var score = enemyBase.KillEnemy();
                _scoreChanged?.OnNext(score);
            }
        }

    }
}
