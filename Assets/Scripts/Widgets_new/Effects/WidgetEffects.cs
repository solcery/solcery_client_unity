using System;
using System.Collections.Generic;
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
        private readonly Stack<RectTransform> _transformsPool;
        
        public static IWidgetEffects Create(RootUiEffects effectRoot)
        {
            return new WidgetEffects(effectRoot);
        }
        
        private WidgetEffects(RootUiEffects effectRoot)
        {
            _effectRoot = effectRoot;
            _transformsPool = new Stack<RectTransform>();
            DOTween.SetTweensCapacity(600, 150);
        }

        private RectTransform PopCardEffectFromPool(Vector2 size)
        {
            if (!_transformsPool.TryPop(out var rectTransform))
            {
                rectTransform = Object.Instantiate(_effectRoot.CardEffect, _effectRoot.transform, false)
                    .GetComponent<RectTransform>();
            }
            
            rectTransform.sizeDelta = size;
            return rectTransform;
        }

        private void PushCardEffectToPool(RectTransform rectTransform)
        {
            _transformsPool.Push(rectTransform);
        }
        
        public void MoveToken(RectTransform rectTransform, 
            Sprite sprite,
            Vector3 from,
            float time,
            Color color,
            Action onMoveComplete)
        {
            var to = rectTransform.transform.position;
            var effect = Object.Instantiate(_effectRoot.TokenEffect, _effectRoot.transform, false)
                .GetComponent<TokenEffectLayout>();
            effect.Image.sprite = sprite;
            effect.transform.position = from;
            effect.RectTransform.sizeDelta = rectTransform.rect.size;
            effect.UpdateEffectColor(color);

            effect.UpdateCreateAnimation(true);
            DOTween.To(_ => { }, 0, 0, 0.2f).OnComplete(() =>
            {
                effect.UpdateCreateAnimation(false);
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
            }).Play();
        }

        public void DestroyToken(RectTransform rectTransform, 
            Sprite sprite,
            float time,
            Color color,
            Action onMoveComplete)
        {
            var effect = Object.Instantiate(_effectRoot.TokenEffect, _effectRoot.transform, false)
                .GetComponent<TokenEffectLayout>();
            effect.Image.sprite = sprite;
            effect.transform.position = rectTransform.transform.position;
            effect.RectTransform.sizeDelta = rectTransform.rect.size;
            effect.UpdateDestroyAnimation(true);
            effect.UpdateEffectColor(color);

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
            PlaceWidgetCardFace cardFace, 
            Vector2 size,
            float time, 
            Vector3 from,
            Vector3 to,
            Action onMoveComplete)
        {
            var effectRectTransform = PopCardEffectFromPool(size);
            effectRectTransform.position = from;
            eclipseCard.UpdateCardFace(cardFace, false);
            eclipseCard.Layout.UpdateParent(effectRectTransform);
            DOTween.Sequence(effectRectTransform.DOMove(to, time).OnComplete(() =>
            {
                PushCardEffectToPool(effectRectTransform);
                onMoveComplete?.Invoke();
            }).SetEase(Ease.Linear)).Play();
        }
        
        public void MoveEclipseCard(RectTransform eclipseCard,
            float time,
            Vector3 from,
            Action onMoveComplete)
        {
            var to = eclipseCard.position;
            var effect = Object.Instantiate(eclipseCard.gameObject, _effectRoot.transform, false)
                .GetComponent<RectTransform>();
            effect.anchorMin = new Vector2(.5f, .5f);
            effect.anchorMax = new Vector2(.5f, .5f);
            effect.position = from;
            effect.sizeDelta = eclipseCard.rect.size;
            effect.gameObject.SetActive(true);
            
            DOTween.Sequence(effect.DOMove(to, time).OnComplete(() =>
            {
                Object.Destroy(effect.gameObject);
                onMoveComplete?.Invoke();
            }).SetEase(Ease.Linear)).Play();
        }
        
        public void DestroyEclipseCard(EclipseCardEffectLayout effectLayout,
            IWidgetRenderData renderData,
            float time,
            Action onMoveComplete)
        {
            effectLayout.gameObject.SetActive(true);
            effectLayout.Image.material.SetFloat("_Destruct", 0f);
            effectLayout.Image.material.SetTexture("_MainTex", renderData.RenderTexture);
            effectLayout.ParticleSystem.Play();

            var alpha = 0f;
            DOTween.Sequence()
                .Append(DOTween.To(() => alpha, x => alpha = x, 1f, time).OnUpdate(() =>
                {
                    effectLayout.Image.material.SetFloat("_Destruct", alpha);
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