using System;
using Player;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int _score;

    private Action _onDeath;

    [SerializeField] private int _damageAmount;
    private bool _dead = true;

    public Vector3 Direction { get; set; }
    void Awake()
    {
       
    }
    
    public virtual void OnFixedUpdate()
    {
        
    }

    public virtual void OnUpdate()
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
}
