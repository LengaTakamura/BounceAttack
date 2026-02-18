using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;
using R3;
using UnityEngine.UI;

public class ResultUi : UiBase
{
    private int _count;
    private Sequence _seq;
    private bool _isHide;
    public override void Init(Presenter presenter)
    {
        _count = 0;
        gameObject.SetActive(false);
        _isHide = true;
        presenter.OnDeathWithScore.Subscribe(score => SetResultUi(score)).AddTo(this);
    }

    public override void UIOnBeat(BeatInfo info)
    {
        if(_isHide) return;
        _count++;
        if (_count % 2 == 0)
        {
            _seq?.Kill();
            _seq = DOTween.Sequence()
                .Append(transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), info.SecondsPerBeat))
                .Append(transform.DOScale(new Vector3(1f, 1f, 1f), info.SecondsPerBeat));
        }
    }

    public void SetResultUi(int score)
    {
        _isHide = false;
        gameObject.SetActive(true);
        var text = gameObject.GetComponentInChildren<Text>();
        text.text = $"Score : {score}";
    }

}
