using System.Collections.Generic;
using CriWare;
using UnityEngine;
using UnityEngine.Rendering;

namespace System
{
    public class InputManager : MonoBehaviour, IBeatSyncListener
    {
        BeatInfo _info;
        private float _prevBeatTime;
        private float _nextBeatTime;
        public InputType CurrentInputType { get; private set; }
        [SerializeField] private SerializableDictionary<InputType, int> _baseScores = new();
        private GameEvents _gameEvents;

        private void Start()
        {
            BeatSyncDispatcher.Instance.Register(this);
            _gameEvents = GameEvents.Instance;
        }

        private void Update()
        {
            CurrentInputType = GetInputType();
            InputHandler();
        }

        private InputType GetInputType()
        {
            if (Input.GetKeyDown(KeyCode.Space)) return InputType.Spase;
            if (Input.GetMouseButtonDown(0)) return InputType.Attack;
            if (Input.GetMouseButtonDown(1)) return InputType.Blink;
            return InputType.None;
        }

        private void InputHandler()
        {
            switch (CurrentInputType)
            {
                case InputType.Spase:
                    var typeSpase = BeatUtility.JudgeBeatAction(_info, _prevBeatTime, _nextBeatTime);
                    _gameEvents.AddScore((int)Score(InputType.Spase, typeSpase));
                    _gameEvents.UpdateInputAction(typeSpase);
                    break;
                case InputType.Attack:
                    var typeAttack = BeatUtility.JudgeBeatAction(_info, _prevBeatTime, _nextBeatTime);
                    _gameEvents.AddScore((int)Score(InputType.Attack, typeAttack));
                    _gameEvents.UpdateInputAction(typeAttack);
                    break;
                case InputType.Blink:
                    var typeBlink = BeatUtility.JudgeBeatAction(_info, _prevBeatTime, _nextBeatTime);
                    _gameEvents.AddScore((int)Score(InputType.Blink, typeBlink));
                    _gameEvents.UpdateInputAction(typeBlink);
                    break;
                case InputType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float Score(InputType inputType, BeatActionType actionType)
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
            return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        }

        public void OnBeat(BeatInfo info)
        {
            UpdateInputInfo(info);
        }

        private void UpdateInputInfo(BeatInfo beatInfo)
        {
            _info = beatInfo;
        }


        private void OnDestroy()
        {
            BeatSyncDispatcher.Instance.Unregister(this);
        }
    }
    
    [Serializable]
    public class SerializableDictionary<TKey, TValue> :
        Dictionary<TKey, TValue>,
        ISerializationCallbackReceiver
    {
        [Serializable]
        public class Pair
        {
            public TKey key = default;
            public TValue value = default;

            /// <summary>
            /// Pair
            /// </summary>
            public Pair(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }

        [SerializeField]
        private List<Pair> _list = null;

        /// <summary>
        /// OnAfterDeserialize
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            foreach (Pair pair in _list)
            {
                if (ContainsKey(pair.key))
                {
                    continue;
                }
                Add(pair.key, pair.value);
            }
        }

        /// <summary>
        /// OnBeforeSerialize
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // 処理なし
        }
    }
    public enum InputType
    {
        None,
        Spase,
        Blink,
        Attack
    }
}


  
    