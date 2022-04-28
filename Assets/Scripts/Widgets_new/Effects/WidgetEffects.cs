using System;
using DG.Tweening;
using Solcery.Services.Renderer.Widgets;
using Solcery.Ui;
using Solcery.Widgets_new.Eclipse.Cards;
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

        public void MoveToken(RectTransform rectTransform, 
            Sprite sprite,
            Vector3 from,
            float time,
            Action onMoveComplete)
        {
            var to = rectTransform.transform.position;
            var effect = Object.Instantiate(_effectRoot.TokenEffect, _effectRoot.transform, false)
                .GetComponent<TokenEffectLayout>();
            effect.Image.sprite = sprite;
            effect.transform.position = from;
            effect.RectTransform.sizeDelta = rectTransform.rect.size;

            effect.RectTransform.DOJump(to, Random.Range(0.5f, 1.5f),0, time).OnComplete(() =>
            {
                Object.Destroy(effect.gameObject);
                onMoveComplete?.Invoke();
            }).SetEase(Ease.Linear).Play();
        }

        public void DestroyToken(RectTransform rectTransform, 
            Sprite sprite,
            float time,
            Action onMoveComplete)
        {
            var effect = Object.Instantiate(_effectRoot.TokenEffect, _effectRoot.transform, false)
                .GetComponent<TokenEffectLayout>();
            effect.Image.sprite = sprite;
            effect.transform.position = rectTransform.transform.position;
            effect.RectTransform.sizeDelta = rectTransform.rect.size;

            DOTween.Sequence()
                .Append(effect.RectTransform.DOScale(new Vector3(1.5f, 1.5f, 1f), time / 2))
                .Append(effect.RectTransform.DOScale(new Vector3(0, 0, 0), time / 2))
                .AppendCallback(() =>
                {
                    Object.Destroy(effect.gameObject);
                    onMoveComplete?.Invoke();
                })
                .Play();
        }

        public void DestroyEclipseCard(IEclipseCardInContainerWidget eclipseCard,
            IWidgetRenderData renderData,
            float time,
            Action onMoveComplete)
        {
            var rect = eclipseCard.Layout.RectTransform.rect;
            var maxSize = Mathf.Max(rect.size.x, rect.size.y);
            var effectLayout = eclipseCard.Layout.EffectLayout;
            effectLayout.Image.texture = renderData.RenderTexture;
            effectLayout.RectTransform.sizeDelta = new Vector2(maxSize, maxSize);
            effectLayout.RectTransform.localPosition = new Vector2((maxSize - rect.size.x) / 2f, 0f);

            effectLayout.CanvasGroup.alpha = 1f;
            var alpha = effectLayout.CanvasGroup.alpha;
            DOTween.Sequence()
                .Append(DOTween.To(() => alpha, x => alpha = x, 0f, time).OnUpdate(() =>
                {
                    effectLayout.CanvasGroup.alpha = alpha;
                }))
                .AppendCallback(() => { onMoveComplete?.Invoke(); })
                .Play();
        }

        public void Destroy()
        {
        }
    }
}