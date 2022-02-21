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

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image background;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI caption;
        [SerializeField] private GameObject borders;

        private int _placeId;
        private int _orderZ;
        private int _linkedEntityId;

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

        public void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        public virtual void UpdateAlpha(int alpha)
        {
        }

        public void UpdateBackgroundColor(string backgroundColor)
        {
            if (backgroundColor != null)
            {
                if (ColorUtility.TryParseHtmlString(backgroundColor, out var bgColor))
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

        public void UpdateOutOfBorder(bool active)
        {
            if (borders != null)
            {
                borders.SetActive(active);
            }
        }

        public void UpdateCaption(string text)
        {
            if (caption != null)
            {
                var active = !string.IsNullOrEmpty(text);
                caption.gameObject.SetActive(active);
                if (active)
                {
                    caption.text = text;
                }
            }
        }
    }
}