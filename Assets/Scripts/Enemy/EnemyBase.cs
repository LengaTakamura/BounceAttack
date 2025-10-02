using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private float _score;

    private Action _onDeath;
    
    private float _timer;
    
    void Awake()
    {
    }
    

    private void Update()
    {
       
    }

    public virtual void EnemyOnBeat(BeatInfo info)
    {
       
    }

    public virtual void Init(BeatInfo beatinfo)
    {
        
    }

    public void InitOnPool(Action release)
    {
        _onDeath += release;
    }
    
    public virtual float Kill()
    {
        _onDeath?.Invoke();
        _onDeath = null;
        return _score;
    }
    
}

public interface IDamageable
{
    float MaxHealth { get; set; }

    float CurrentHealth { get; set; }

    void HitDamage(float damage);

    void HitHeal(float value);

    void Kill();

    float AttackPower {  get; set; }
    
}
