using R3;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private int _currentHealth;

        [SerializeField] private int _maxHealth;

        [SerializeField] private float _attackRadius;
    
        [SerializeField] private LayerMask _enemyLayerMask;
    
        private readonly Subject<Unit> _onHit = new();
        public Observable<Unit> OnHit => _onHit;
        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _onHit?.OnNext(Unit.Default);
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
