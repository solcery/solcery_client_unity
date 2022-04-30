using System;
using DG.Tweening;
using Solcery.Widgets_new.Canvas;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public class EclipseCardFullModeController
    {
        private EclipseCardFullModeLayout _fullModeLayout;
        private EclipseCardInContainerWidgetLayout _clone;
        private EclipseCardInContainerWidgetLayout _eclipseCard;
        private Sequence _sequence;
        
        public static EclipseCardFullModeController Create(IWidgetCanvas widgetCanvas)
        {
            return new EclipseCardFullModeController(widgetCanvas);
        }

        private EclipseCardFullModeController(IWidgetCanvas widgetCanvas)
        {
            _fullModeLayout = widgetCanvas.GetFullModeLayout();
            _fullModeLayout.BackgroundButton.onClick.AddListener(Hide);
        }

        public void Show(EclipseCardInContainerWidgetLayout eclipseCard)
        {
            _eclipseCard = eclipseCard;
            _fullModeLayout.gameObject.SetActive(true);
            var obj = Object.Instantiate(_eclipseCard.gameObject, _fullModeLayout.PlaceRectTransform, false);
            _clone = obj.GetComponent<EclipseCardInContainerWidgetLayout>();
            _clone.RectTransform.sizeDelta = new Vector2(0, _eclipseCard.RectTransform.rect.height);
            _clone.RectTransform.anchorMin = new Vector2(.5f, .5f);
            _clone.RectTransform.anchorMax = new Vector2(.5f, .5f);
            _clone.transform.position = _eclipseCard.RectTransform.position;
            _clone.RaycastOff();
            _eclipseCard.SetActive(false);
            PlayAnimation(_clone.RectTransform, 0.3f, _fullModeLayout.PlaceRectTransform.rect.height, _fullModeLayout.transform.position);
        }

        private void Hide()
        {
            PlayAnimation(_clone.RectTransform, 0.3f, _eclipseCard.RectTransform.rect.height, _eclipseCard.RectTransform.position,
                () =>
                {
                    _fullModeLayout.gameObject.SetActive(false);
                    _eclipseCard.SetActive(true);
                    Cleanup();
                });
        }

        private void Cleanup()
        {
            if (_clone != null)
            {
                Object.Destroy(_clone.gameObject);
                _clone = null;
                _eclipseCard = null;
            }
        }

        public void Destroy()
        {
            Cleanup();
            _fullModeLayout.BackgroundButton.onClick.RemoveListener(Hide);
            _fullModeLayout = null;
        }

        private void PlayAnimation(RectTransform rectTransform, float time, float heightTo, Vector3 positionTo,
            Action onComplete = null)
        {
            if (_sequence != null)
            {
                return;
            }
            
            var height = rectTransform.rect.height;
            _sequence = DOTween.Sequence()
                .Append(DOTween.To(() => height, x => height = x, heightTo, time).OnUpdate(() =>
                {
                    rectTransform.sizeDelta = new Vector2(0, height);
                }))
                .Join(rectTransform.transform.DOMove(positionTo, time)).SetEase(Ease.InFlash)
                .AppendCallback(() =>
                {
                    _sequence.Kill();
                    _sequence = null;
                    onComplete?.Invoke();
                })
                .Play();
        }
    }
}
