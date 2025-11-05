using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ActionUi : UiBase
    {
        private TextMeshProUGUI _text;

        private Sequence _seq;

        [SerializeField] private float _fadeTime = 0.25f;

        public override void Init(Presenter presenter)
        {
            _text = GetComponent<TextMeshProUGUI>();
            presenter.OnInputAction.Subscribe(OnActionTriggered).AddTo(this);
        }

        public override void UIOnBeat(BeatInfo info)
        {
        }

        private void OnActionTriggered(BeatActionType action)
        {
            switch (action)
            {
                case BeatActionType.Bad:
                {
                    _text.text = "Bad...";
                    break;
                }
                case BeatActionType.Good:
                {
                    _text.text = "Good!";
                    break;
                }
                case BeatActionType.Great:
                {
                    _text.text = "Great!!!";
                    break;
                }
            }

            _seq?.Kill();
            _seq = DOTween.Sequence().Append(_text.transform.DOScale(2f, _fadeTime)
                    .SetEase(Ease.OutBounce))
                .Join(_text.DOFade(1f, _fadeTime))
                .OnComplete(OnComplete);
        }

        private void OnComplete()
        {
            _text.DOFade(0f, _fadeTime);
            _text.transform.DOScale(1f, _fadeTime);
        }
    }
}