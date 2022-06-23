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
            var effectLayout = eclipseCard.Layout.EffectLayout;
            effectLayout.gameObject.SetActive(true);
            effectLayout.Image.material.SetFloat("_Destruct", 0f);
            effectLayout.Image.material.SetTexture("_MainTex_RT", renderData.RenderTexture);
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