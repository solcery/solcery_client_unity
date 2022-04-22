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

        public void DestroyEclipseCard(RectTransform rectTransform, 
            RenderTexture rtt,
            float time, 
            Action onMoveComplete)
        {
            var effect = Object.Instantiate(_effectRoot.EclipseCardEffect, _effectRoot.transform, false)
                .GetComponent<EclipseCardEffectLayout>();
            var rect = rectTransform.rect;
            var maxSize = Mathf.Max(rect.size.x, rect.size.y);
            effect.RectTransform.sizeDelta = new Vector2(maxSize, maxSize);
            effect.RectTransform.position = rectTransform.position;
            effect.RectTransform.anchoredPosition += new Vector2( rect.width / 2f, 0f);
            effect.Image.texture = rtt;
            var alpha = effect.CanvasGroup.alpha;
            DOTween.Sequence()
                .Append(DOTween.To(() => alpha, x => alpha = x, 0f, time).OnUpdate (() =>
                {
                    effect.CanvasGroup.alpha = alpha;
                }))
                .AppendCallback(() =>
                {
                    Object.Destroy(effect.gameObject);
                    onMoveComplete?.Invoke();
                })
                .Play();
        }

        public void Destroy()
        {
        }
    }
}