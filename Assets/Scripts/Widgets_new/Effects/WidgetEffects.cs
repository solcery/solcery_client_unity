using System;
using DG.Tweening;
using Solcery.Ui;
using Solcery.Widgets_new.Eclipse.Effects;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Solcery.Widgets_new.Effects
{
    public class WidgetEffects : IWidgetEffects
    {
        private readonly RootUiEffects _effectRoot;
        
        public static IWidgetEffects Create(RootUiEffects effectRoot)
        {
            return new WidgetEffects(effectRoot);
        }
        
        private WidgetEffects(RootUiEffects effectRoot)
        {
            _effectRoot = effectRoot;
        }

        public void MoveToken(Sprite sprite,
            Vector2 sizeDelta,
            Vector3 from,
            Vector3 to,
            float time,
            Action onMoveComplete)
        {
            var effect = Object.Instantiate(_effectRoot.TokenEffect, _effectRoot.transform, false).GetComponent<TokenEffectLayout>();
            effect.Image.sprite = sprite;
            effect.transform.position = from;
            effect.RectTransform.sizeDelta = sizeDelta;

            var parabolaHeight = Random.Range(-1.0f, 1.0f);
            DOTween.To(value => { effect.transform.position = Parabola(from, to, parabolaHeight, value); }, 0, 1, time)
                .OnComplete(() =>
                {
                    Object.Destroy(effect.gameObject);
                    onMoveComplete?.Invoke();
                })
                .SetEase(Ease.OutFlash);
        }

        public void Destroy()
        {
        }

        private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            float Func(float x) => 4 * (-height * x * x + height * x);
            var mid = Vector3.Lerp(start, end, t);
            return new Vector3(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }
}