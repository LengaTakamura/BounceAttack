using System;
using Player;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int _score;

    private Action _onDeath;
    
    private float _timer;

    [SerializeField] private int _damageAmount;

    [SerializeField] private float _speed;

    private bool _dead = true;
    void Awake()
    {
       
    }
    

    private void Update()
    {
    }

    public virtual void EnemyOnBeat(BeatInfo info)
    {
       
    }

    public virtual void Init(BeatInfo beatInfo)
    {
        
    }

    public void InitOnPool(Action release)
    {
        _onDeath += release;
        _dead = false;
    }
    
    protected void Suicide()
    {
        if (_dead) return;
        _dead = true;
        _onDeath?.Invoke();
        _onDeath = null;
    }

    public int KillEnemy()
    {
        if (_dead) return 0;
        _dead = true;
        _onDeath?.Invoke();
        _onDeath = null;
        return _score;
    } 
    public virtual void OnAttack(PlayerManager player)
    {
        player.TakeDamage(_damageAmount);
    }

    public int GetDamageAmount()
    {
        return _damageAmount;
    }

    public virtual void Move(Vector3 destination)
    {
        var vect = (destination - transform.position).normalized;
        transform.Translate(vect * _speed);
    }
}
