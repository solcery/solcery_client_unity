using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.CardFull;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Tooltip
{
    public class TooltipLayout : MonoBehaviour
    {
        public RectTransform RectTransform;
        [SerializeField] private ContentSizeFitter contentSizeFitter;
        [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField] private TextMeshProUGUI simpleText;
        [SerializeField] private RectTransform simpleTextRectTransform;
        [SerializeField] private Image background;
        [SerializeField] private PlaceWidgetEclipseCardFullLayout eclipseCard;

        public void ShowEclipseCard(IGame game, JObject cardTypeDataObject)
        {
            if (cardTypeDataObject.TryGetEnum(GameJsonKeys.CardType, out EclipseCardTypes eclipseCardType))
            {
                contentSizeFitter.enabled = false;
                horizontalLayoutGroup.enabled = false;
                eclipseCard.UpdateCardType(game, eclipseCardType, cardTypeDataObject);
                eclipseCard.TokensLayout.UpdateTokenSlots(0);
                eclipseCard.TimerLayout.gameObject.SetActive(false);
                eclipseCard.gameObject.SetActive(true);
            }
        }

        public void ShowSimpleText(JObject tooltipDataObject)
        {
            if (simpleText == null)
                return;
            
            contentSizeFitter.enabled = true;
            horizontalLayoutGroup.enabled = true;
            simpleText.text = tooltipDataObject.GetValue<string>(GameJsonKeys.TooltipText);
            simpleText.fontSize = tooltipDataObject.TryGetValue(GameJsonKeys.TooltipFontSize, out int fontSizeAttribute) ? fontSizeAttribute : 36;;
            simpleText.gameObject.SetActive(true);
        }

        public void UpdateFillColor(JObject tooltipDataObject)
        {
            var colorSet = false;
            var fillColor = tooltipDataObject.TryGetValue(GameJsonKeys.TooltipFillColor, out string fillColorAttribute) ? fillColorAttribute : null;
            if (fillColor != null)
            {
                if (ColorUtility.TryParseHtmlString(fillColor, out var bgColor))
                {
                    if (background != null)
                    {
                        background.color = bgColor;
                        colorSet = true;
                    }
                }
            }

            if (background != null)
            {
                background.gameObject.SetActive(colorSet);
            }
        }

        public void HideContent()
        {
            simpleText.gameObject.SetActive(false);
            eclipseCard.gameObject.SetActive(false);
        }
        
        public void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform.anchorMin = anchorMin;
            RectTransform.anchorMax = anchorMax;
        }

        public void UpdateOffset(Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform.offsetMin = offsetMin;
            RectTransform.offsetMax = offsetMax;
        }
        
        public void ToDefaultAnchors()
        {
            RectTransform.anchoredPosition = Vector2.zero;
            UpdateAnchor(Vector2.zero, Vector2.zero);
            UpdateOffset(Vector2.zero, Vector2.zero);
        }

        public void RebuildLayouts()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(simpleTextRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        }
    }
}