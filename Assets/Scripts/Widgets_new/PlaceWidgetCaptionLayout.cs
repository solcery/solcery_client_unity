using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new
{
    public class PlaceWidgetCaptionLayout : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI caption;
        [SerializeField] private RectTransform placeInside;
        [SerializeField] private RectTransform placeAbove;
        [SerializeField] private RectTransform placeCentered;
        
        public void UpdateCaption(string text, PlaceCaptionPosition position)
        {
            if (caption != null)
            {
                var active = !string.IsNullOrEmpty(text);
                caption.gameObject.SetActive(active);
                if (active)
                {
                    caption.text = text;
                    SetCaptionType(position);
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

        private void SetCaptionType(PlaceCaptionPosition position)
        {
            switch (position)
            {
                case PlaceCaptionPosition.Above:
                    caption.transform.SetParent(placeAbove, false);
                    break;
                case PlaceCaptionPosition.Inside:
                    caption.transform.SetParent(placeInside, false);
                    break;
                case PlaceCaptionPosition.Centered:
                    caption.transform.SetParent(placeCentered, false);
                    break;
            }
        }
    }
}