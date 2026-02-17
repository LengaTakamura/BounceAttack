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

        private PlayerMove _move;
        public void InGameInit(InputManager inputManager)
        {
            _currentHealth = _maxHealth;
            _move = GetComponent<PlayerMove>();
            _move.Init(inputManager);
        }

        public void TakeDamage(int damage)
        {
            if (_move.IsBlinking) return; 
            _currentHealth -= damage;
            _onHit?.OnNext(damage);
        }

        public void AttackEnemies()
        {
            var array = new Collider[100];
            var count = Physics.OverlapSphereNonAlloc(transform.position, _attackRadius, array, _enemyLayerMask);
            for (int i = 0; i < count; i++)
            {
                if(!array[i].gameObject.TryGetComponent(out EnemyBase enemyBase)) return;
                enemyBase.KillEnemy();
            }
        }
    
    }
}
