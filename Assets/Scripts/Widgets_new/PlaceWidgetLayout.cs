using Solcery.Widgets_new.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new
{
    public class PlaceWidgetLayout : MonoBehaviour
    {
        public int PlaceId => _placeId;
        public int OrderZ => _orderZ;
        public int LinkedEntityId => _linkedEntityId;
        public PlaceWidget PlaceWidget => _placeWidget;
        public bool BlockRaycasts => canvasGroup.blocksRaycasts;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image background;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] private PlaceWidgetCaptionLayout caption;
        [SerializeField] private Image frame;
        [SerializeField] private PlaceWidgetTimerLayout timer;

        private int _placeId;
        private int _orderZ;
        private int _linkedEntityId;
        private PlaceWidget _placeWidget;
        private IPlaceWidgetTimer _widgetTimer;
        
        public void UpdatePlaceWidget(PlaceWidget placeWidget)
        {
            _placeWidget = placeWidget;
        }

        public void UpdateVisible(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public void UpdatePlaceId(int placeId)
        {
            _placeId = placeId;
        }

        public void UpdateOrderZ(int orderZ)
        {
            _orderZ = orderZ;
        }

        public void UpdateLinkedEntityId(int linkedEntityId)
        {
            _linkedEntityId = linkedEntityId;
        }

        public void UpdateSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }

        public virtual void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        public virtual void UpdateAlpha(int alpha)
        {
        }
        
        public void UpdateFillColor(string fillColor)
        {
            if (fillColor != null)
            {
                if (ColorUtility.TryParseHtmlString(fillColor, out var bgColor))
                {
                    if (background != null)
                    {
                        background.gameObject.SetActive(true);
                        background.color = bgColor;
                        return;
                    }
                }
            }

            if (background != null)
            {
                background.gameObject.SetActive(false);
            }
        }

        public void UpdateFrameActive(bool active)
        {
            if (frame != null)
            {
                frame.gameObject.SetActive(active);
            }
        }

        public void UpdateFrameColor(string frameColor)
        {
            if (frameColor != null)
            {
                if (ColorUtility.TryParseHtmlString(frameColor, out var color))
                {
                    if (frame != null)
                    {
                        frame.color = color;
                    }
                }
            }
        }
        
        public void UpdateCaption(string text, PlaceCaptionPosition position, float fontSize)
        {
            if (caption != null)
            {
                caption.UpdateCaption(text, position, fontSize);
            }
        }

        public void UpdateCaptionColor(string captionColor)
        {
            if (caption != null)
            {
                caption.UpdateCaptionColor(captionColor);
            }
        }
        
        public void UpdateBlocksRaycasts(bool blocksRaycasts)
        {
            canvasGroup.blocksRaycasts = blocksRaycasts;
        }
        
        public void StartTimer(int durationMsec)
        {
            _widgetTimer ??= PlaceWidgetTimer.Create(timer);
            _widgetTimer?.Start(durationMsec);
        }

        public void UpdateTimer(int durationMsec)
        {
            _widgetTimer?.Update(durationMsec);
        }
        
        public void StopTimer()
        {
            _widgetTimer?.Stop();
        }
    }
}