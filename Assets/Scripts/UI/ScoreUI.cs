using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        private TextMeshProUGUI _valueText;

        private Sequence _seq;

        private void Start()
        {
            _valueText = GetComponent<TextMeshProUGUI>();
        }

        private void UpdateScoreText(int score ,float value)
        {
            int currentScore = score;
            int result = score + (int)value;
            _seq?.Kill();
            _seq = DOTween.Sequence().Append(DOTween.To(()=> currentScore, update => currentScore = update, result,0.5f)
                .SetEase(Ease.OutBounce)
                .OnUpdate(() => _valueText.text = currentScore.ToString("D5")));
        }
    }
}
