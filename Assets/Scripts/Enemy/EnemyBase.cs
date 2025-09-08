using System;
using CriWare;
using UnityEngine;

public class EnemyBase : MonoBehaviour,IBeatSyncListener
{
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float AttackPower { get; set; }
    [SerializeField] private float _score;

    private Action _onDeath;

    public virtual void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        
    }

    public void InitOnPool(Action release)
    {
        _onDeath += release;
    }
    
    public virtual float Kill()
    {
        _onDeath?.Invoke();
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
