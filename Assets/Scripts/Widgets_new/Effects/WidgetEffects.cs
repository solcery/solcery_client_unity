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
            effect.UpdateMoveAnimation(true);

            effect.RectTransform.DOJump(to, Random.Range(1f, 2.0f),0, time).OnComplete(() =>
            {
                effect.UpdateMoveAnimation(false);
                Object.Destroy(effect.gameObject);
                onMoveComplete?.Invoke();
            }).SetEase(Ease.Linear).Play();
            
            DOTween.Sequence()
                .Append(effect.RectTransform.DOScale(new Vector3(3f, 3f, 1f), time / 2))
                .Append(effect.RectTransform.DOScale(new Vector3(1f, 1f, 1f), time / 2))
                .Play();

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
            effect.UpdateDestroyAnimation(true);

            DOTween.Sequence()
                .Append(effect.RectTransform.DOScale(new Vector3(1.5f, 1.5f, 1f), time / 2))
                .Append(effect.RectTransform.DOScale(new Vector3(0, 0, 0), time / 2))
                .AppendCallback(() =>
                {
                    effect.UpdateDestroyAnimation(false);
                    Object.Destroy(effect.gameObject);
                    onMoveComplete?.Invoke();
                })
                .Play();
        }

        public void MoveEclipseCard(IEclipseCardInContainerWidget eclipseCard,
            Transform parent,
            float time,
            Vector3 from,
            Action onMoveComplete)
        {
            var to = eclipseCard.Layout.RectTransform.position;
            var effect = Object.Instantiate(eclipseCard.Layout, parent, false)
                .GetComponent<EclipseCardInContainerWidgetLayout>();
            effect.RectTransform.anchorMin = new Vector2(.5f, .5f);
            effect.RectTransform.anchorMax = new Vector2(.5f, .5f);
            effect.RectTransform.position = from;
            effect.RectTransform.sizeDelta = eclipseCard.Layout.RectTransform.rect.size;
            effect.SetActive(true);
            
            DOTween.Sequence()
                .Append(effect.RectTransform.DOMove(to, time))
                .AppendCallback(() =>
                {
                    Object.Destroy(effect.gameObject);
                    onMoveComplete?.Invoke();
                })
                .Play();
            
            
            DOTween.Sequence(effect.RectTransform.DOMove(to, time).OnComplete(() =>
            {
                Object.Destroy(effect.gameObject);
                onMoveComplete?.Invoke();
            }).SetEase(Ease.Linear)).Play();
        }

        public void DestroyEclipseCard(IEclipseCardInContainerWidget eclipseCard,
            IWidgetRenderData renderData,
            float time,
            Action onMoveComplete)
        {
            var rect = eclipseCard.Layout.RectTransform.rect;
            var maxSize = Mathf.Max(rect.size.x, rect.size.y);
            var effectLayout = eclipseCard.Layout.EffectLayout;
            effectLayout.gameObject.SetActive(true);
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
                .AppendCallback(() =>
                {
                    effectLayout.gameObject.SetActive(false);
                    onMoveComplete?.Invoke();
                })
                .Play();
        }

        public void Destroy()
        {
        }
    }
}