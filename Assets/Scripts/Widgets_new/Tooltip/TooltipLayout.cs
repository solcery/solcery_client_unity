using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Tooltip
{
    public class TooltipLayout : MonoBehaviour
    {
        public RectTransform RectTransform;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image background;

        public void UpdateText(string text)
        {
            description.text = text;
        }

        public void UpdateFillColor(string fillColor)
        {
            if (fillColor != null)
            {
                if (ColorUtility.TryParseHtmlString(fillColor, out var bgColor))
                {
                    if (background != null)
                    {
                        background.color = bgColor;
                    }
                }
            }
        }

        public void UpdateFontSize(int fontSize)
        {
            description.fontSize = fontSize;
        }
    }
}