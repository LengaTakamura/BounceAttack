using System;
using System.Collections.Generic;
using CriWare;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour,IBeatSyncListener
{
    public Action OnClick;
    public Action OnPressSpace;
    BeatInfo _info;
    private float _prevBeatTime;
    private float _nextBeatTime;
    public InputType CurrentInputType;
    private void Start()
    {
        var beatSystem = BeatSyncDispatcher.Instance.Get<BeatSystem>();
        beatSystem.OnBeatAction += UpdateInputInfo;
    }

    private void UpdateInputInfo(BeatInfo beatInfo)
    {
        _info = beatInfo;
        _prevBeatTime = _info.NowTime;
        _nextBeatTime = _info.NowTime + _info.SecondsPerBeat;
    }

    private void Update()
    {
        CurrentInputType = GetInputType();
        InputHandler();
    }

    private InputType GetInputType()
    {
        if (Input.GetKeyDown(KeyCode.Space)) return InputType.Spase;
        if(Input.GetMouseButtonDown(0)) return InputType.Attack;
        if(Input.GetMouseButtonDown(1)) return InputType.Blink;
        return InputType.None;
    }

    private void InputHandler()
    {
        switch (CurrentInputType)
        {
            case InputType.Spase:
                var typeSpase = BeatUtility.JudgeBeatAction(_info,_prevBeatTime,_nextBeatTime);
                break;
            case InputType.Attack:
                var typeAttack = BeatUtility.JudgeBeatAction(_info,_prevBeatTime,_nextBeatTime);
                break;
            case InputType.Blink:
                var typeBlink = BeatUtility.JudgeBeatAction(_info,_prevBeatTime,_nextBeatTime);
                break;
            case InputType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Vector3 GetMoveDirection()
    {
        return new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")).normalized;
    }
    
    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
      
    }
}



public enum InputType
{
    None,Spase,Blink,Attack
}
