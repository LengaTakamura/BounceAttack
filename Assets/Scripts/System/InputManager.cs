using CriWare;
using UnityEngine;

namespace System
{
    public class InputManager : MonoBehaviour,IBeatSyncListener
    {
        public Action OnClick;
        public Action OnPressSpace;
        BeatInfo _info;
        private float _prevBeatTime;
        private float _nextBeatTime;
        public InputType CurrentInputType {get; private set;}
        [SerializeField] private SerializedDictionary<InputType, int> _baseScores = new();
        private int _count;
        private void Awake()
        {
            BeatSyncDispatcher.Instance.Register(this);
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
                    GameEvents.Instance.AddScore((int)Score(InputType.Spase, typeSpase));
                    break;
                case InputType.Attack:
                    var typeAttack = BeatUtility.JudgeBeatAction(_info,_prevBeatTime,_nextBeatTime);
                    GameEvents.Instance.AddScore((int)Score(InputType.Attack, typeAttack));
                    break;
                case InputType.Blink:
                    var typeBlink = BeatUtility.JudgeBeatAction(_info,_prevBeatTime,_nextBeatTime);
                    GameEvents.Instance.AddScore((int)Score(InputType.Blink, typeBlink));
                    break;
                case InputType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float Score(InputType inputType,BeatActionType actionType)
        {
            switch (actionType)
            {
                case BeatActionType.None:
                {
                    return 0;
                }
                case BeatActionType.Bad:
                {
                    return _baseScores[inputType] * 0.5f;
                }
                case BeatActionType.Good:
                {
                    return _baseScores[inputType];
                }
                case BeatActionType.Great:
                {
                    return _baseScores[inputType] * 1.5f;
                }
            }
            return 0;
        }

        public Vector3 GetMoveDirection()
        {
            return new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")).normalized;
        }
    
        public void OnBeat(BeatInfo info)
        {
            UpdateInputInfo(info);
        }
        
        private void UpdateInputInfo(BeatInfo beatInfo)
        {
            _info = beatInfo;
            _count++;
            if (_count % 2 == 0)
            {
                _prevBeatTime = beatInfo.NowTime;
                _nextBeatTime = beatInfo.NowTime + beatInfo.SecondsPerBeat;
            }
        }


        private void OnDestroy()
        {
            BeatSyncDispatcher.Instance.Unregister(this);
        }
    }



    public enum InputType
    {
        None,Spase,Blink,Attack
    }
}