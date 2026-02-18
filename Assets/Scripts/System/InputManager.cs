using System.Collections.Generic;
using R3;
using UnityEngine;

namespace System
{
    public class InputManager : IBeatSyncListener,IDisposable
    {
        BeatInfo _info;
        public InputType CurrentInputType { get; private set; }

        InputManagerData _data;
        private InGameBeatSystem _beatSystem;
        
        #region イベント

        private readonly CompositeDisposable _disposables = new();
        private readonly ReactiveProperty<int> _currentScore = new(0);
        public ReadOnlyReactiveProperty<int> CurrentScore => _currentScore;
        
        private readonly Subject<int> _scoreChanged = new();
        public Observable<int> ScoreChanged => _scoreChanged; 

        private readonly Subject<BeatActionType> _onInputAction = new();
        public Observable<BeatActionType> OnInputAction => _onInputAction;
        
        private readonly Subject<Unit> _onAttack = new();

        public Observable<Unit> OnAttack => _onAttack;
        #endregion 

        public InputManager(InGameBeatSystem beatSystem)
        {
            _beatSystem = beatSystem;
            BeatSyncDispatcher.Instance.RegisterBeatSync(this);
        }

        public void InGameInit(InputManagerData data)
        {
            CurrentInputType = InputType.None;
            _data = data;
        }

        public void OnUpdate(bool isDead)
        {
            CurrentInputType = GetInputType();
            if(isDead) return;
            if(_beatSystem.IsWaiting) return;
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
                    var typeSpase = BeatUtility.JudgeBeatAction(_info);
                    AddScore((int)GetScore(InputType.Spase, typeSpase));
                    UpdateInputAction(typeSpase);
                    break;
                case InputType.Attack:
                    var typeAttack = BeatUtility.JudgeBeatAction(_info);
                    AddScore((int)GetScore(InputType.Attack, typeAttack));
                    UpdateInputAction(typeAttack);
                    _onAttack?.OnNext(Unit.Default);
                    break;
                case InputType.Blink:
                    var typeBlink = BeatUtility.JudgeBeatAction(_info);
                    AddScore((int)GetScore(InputType.Blink, typeBlink));
                    UpdateInputAction(typeBlink);
                    break;
                case InputType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void AddScore(int amount)
        {
            _currentScore.Value += amount;
            _scoreChanged.OnNext(amount);
        }
        
        private void UpdateInputAction(BeatActionType action)
        {
            _onInputAction.OnNext(action);
        }

        private float GetScore(InputType inputType, BeatActionType actionType)
        {
            switch (actionType)
            {
                case BeatActionType.None:
                {
                    return 0;
                }
                case BeatActionType.Bad:
                {
                    return _data.BaseScores[inputType] * _data.BadMultiplier;
                }
                case BeatActionType.Good:
                {
                    return _data.BaseScores[inputType] * _data.GoodMultiplier;
                }
                case BeatActionType.Great:
                {
                    return _data.BaseScores[inputType] * _data.GreatMultiplier;
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
        public void Dispose()
        {
            BeatSyncDispatcher.Instance.UnregisterBeatSync(this);

            _disposables?.Dispose();

            _currentScore?.Dispose();
            _scoreChanged?.Dispose();
            _onInputAction?.Dispose();
            _onAttack?.Dispose();
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


  
    