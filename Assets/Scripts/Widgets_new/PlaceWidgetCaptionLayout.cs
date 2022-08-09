using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new
{
    public class PlaceWidgetCaptionLayout : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI caption;
        [SerializeField] private RectTransform placeInside;
        [SerializeField] private RectTransform placeAbove;
        
        public void UpdateCaption(string text, PlaceCaptionType type)
        {
            if (caption != null)
            {
                var active = !string.IsNullOrEmpty(text);
                caption.gameObject.SetActive(active);
                if (active)
                {
                    caption.text = text;
                    SetCaptionType(type);
                }
            }
        }

        public void UpdateCaptionColor(string captionColor)
        {
            if (captionColor != null)
            {
                if (ColorUtility.TryParseHtmlString(captionColor, out var color))
                {
                    if (caption != null)
                    {
                        caption.color = color;
                    }
                }
            }        
        }

        private void SetCaptionType(PlaceCaptionType type)
        {
            switch (type)
            {
                case PlaceCaptionType.Above:
                    caption.transform.SetParent(placeAbove, false);
                    break;
                case PlaceCaptionType.Inside:
                    caption.transform.SetParent(placeInside, false);
                    break;
            }
        }
    }
}