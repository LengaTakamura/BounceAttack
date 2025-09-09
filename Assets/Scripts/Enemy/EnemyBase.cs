using System;
using CriWare;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    [SerializeField] private float _score;

    private Action _onDeath;

    private BeatInfo _beatInfo;
    
    void Awake()
    {
    }
    
    private void OnDisable()
    {

    }
    
    public void UpdateInfo(BeatInfo info)
    {
        _beatInfo = info;
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
