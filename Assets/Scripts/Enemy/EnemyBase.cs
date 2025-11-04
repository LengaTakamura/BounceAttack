using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int _score;

    private Action _onDeath;
    
    private float _timer;

    [SerializeField] private int _damageAmount;
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
    }
    
    protected void Suicide()
    {
        _onDeath?.Invoke();
        _onDeath = null;
    }

    public int KillEnemy()
    {
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
