using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HpBarUi : UiBase
    {
        [SerializeField] private Image _burnImage;
        [SerializeField] private Image _healthImage;
        [SerializeField] private float _duration = 0.5f;
        private float _strength = 20f;
        private int _vibrate = 100;
        private float _debugDamage = 10f;
        private float _health = 100f;
        private float _maxHealth = 100f;
        public override void Init(Presenter presenter)
        {
            _burnImage.fillAmount = 1f;
            _healthImage.fillAmount = 1f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(_debugDamage);
            }
        }

        private void TakeDamage(float damage)
        {
            SetGauge((_health - damage) / _maxHealth);
            _health -= damage;
        }
        
        

        /// <summary>
        /// HpBarの演出部分の実装　
        /// </summary>
        /// <param name="targetValue">HPバーが最終的に到達するべき目標値(割合)</param>
        public void SetGauge(float targetValue)
        {
            transform.DOShakePosition(_duration/ 2,_strength, _vibrate);
            _healthImage.DOFillAmount(targetValue, _duration).OnComplete(() =>
            {
                _burnImage.DOFillAmount(targetValue, _duration / 2);
            });
        }

        public override void UIOnBeat(BeatInfo info)
        {
           
        }
    }
}
