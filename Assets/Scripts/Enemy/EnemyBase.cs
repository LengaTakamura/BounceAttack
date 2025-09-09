using System;
using System.Threading;
using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private float _score;

    private Action _onDeath;

    protected BeatInfo BeatInfo;
    
    private float _timer;
    void Awake()
    {
    }

    private async void Start()
    {
        var cts = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(10f),cancellationToken: cts.Token);
        Kill();
        cts.Cancel();
        cts.Dispose();
    }


    private void Update()
    {
       
    }

    public virtual void EnemyOnBeat(BeatInfo info)
    {
        BeatInfo = info;
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
