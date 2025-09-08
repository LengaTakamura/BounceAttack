using System;
using CriWare;
using UnityEngine;

public class EnemyBase : MonoBehaviour,IBeatSyncListener,IDamageable
{
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float AttackPower { get; set; }
    
    public Action OnDeath {get ; set ;}

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        
    }

    public void InitOnPool(Action release)
    {
        OnDeath += release;
    }

    
    public void HitDamage(float damage)
    {
        
    }

    public void HitHeal(float value)
    {
       
    }

    public void Kill()
    {
        OnDeath?.Invoke();
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
