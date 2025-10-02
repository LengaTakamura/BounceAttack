using System;
using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUi : UiBase
    {
        private TextMeshProUGUI _valueText;

        private Sequence _seq;
        
        private int _previousScore;
        public override void Init(GameEvents gameEvents)
        {
            _previousScore = 0;
            _valueText = GetComponent<TextMeshProUGUI>();
            gameEvents.CurrentScore.Pairwise().Subscribe(value => UpdateScoreText(value.Previous, value.Current));
        }

        public override void UIOnBeat(BeatInfo info)
        {
            
        }

        private void UpdateScoreText(int previous,int current)
        {
            var score = previous;
            _seq?.Kill();
            _seq = DOTween.Sequence().Append(DOTween
                    .To(() => score, update => score = update, current, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .OnUpdate(() => _valueText.text = score.ToString("D5")))
                .OnComplete(() =>OnComplete(current));
        }

        private void OnComplete(int current)
        {
            _valueText.text = current.ToString("D5");
        }
        
    }
}
