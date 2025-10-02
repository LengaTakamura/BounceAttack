using R3;
using UnityEngine;

namespace System
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents Instance;
        
        #region イベント
        private readonly Subject<int> _scoreChanged = new();
        public Observable<int> ScoreChanged => _scoreChanged; 
        
        private readonly ReactiveProperty<int> _currentScore = new(0);
        public ReadOnlyReactiveProperty<int> CurrentScore => _currentScore;
        #endregion 

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddScore(int amount)
        {
            _currentScore.Value += amount;
            _scoreChanged.OnNext(amount);
        }
        
        public void OnDestroy()
        {
            _scoreChanged.Dispose();
            _currentScore.Dispose();
        }
    }
}
