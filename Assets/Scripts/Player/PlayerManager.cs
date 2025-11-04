using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private int _currentHealth;

    [SerializeField] private int _maxHealth;

    [SerializeField] private float _attackRadius;
    
    [SerializeField] private LayerMask _enemyLayerMask;
    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
    }

    public void AttackEnemies()
    {
        var array = new Collider[100];
        var count = Physics.OverlapSphereNonAlloc(transform.position, _attackRadius, array, _enemyLayerMask);
    }
}
