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
            var effect = Object.Instantiate(_effectRoot.TokenEffect, _effectRoot.transform, false)
                .GetComponent<TokenEffectLayout>();
            effect.Image.sprite = sprite;
            effect.transform.position = from;
            effect.RectTransform.sizeDelta = sizeDelta;

            effect.RectTransform.DOJump(to, Random.Range(0.5f, 1.5f),0, time).OnComplete(() =>
            {
                Object.Destroy(effect.gameObject);
                onMoveComplete?.Invoke();
            }).SetEase(Ease.Linear).Play();
        }

        public void Destroy()
        {
        }
    }
}