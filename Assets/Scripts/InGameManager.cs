using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameManager : MonoBehaviour
{
    private StateBase _currentState;


    public void SetState<TState>() where TState : StateBase, new()
    {
        _currentState?.Exit(this);
        var nextState = new TState();
        _currentState = nextState;
        _currentState?.Enter(this);
    }

    private void Start()
    {
        SetState<Preparation>();
    }
    private void Update()
    {
        _currentState?.Update(this);
    }
    private void FixedUpdate()
    {
        _currentState?.FixedUpdate(this);
    }

    private async UniTask InGameTimer()
    {
        
    }
    
    
}

