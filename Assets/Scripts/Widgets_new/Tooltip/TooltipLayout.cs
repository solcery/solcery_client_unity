using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
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
        [SerializeField] private TextMeshProUGUI simpleText;
        [SerializeField] private RectTransform simpleTextRectTransform;
        [SerializeField] private Image background;
        [SerializeField] private PlaceWidgetEclipseCardFullLayout eclipseCard;

        public void ShowEclipseCard(IGame game, IItemType itemType)
        {
            if (itemType.TryGetValue(out var valueToken, GameJsonKeys.CardType)
                & valueToken.TryGetEnum(out EclipseCardTypes eclipseCardType))
            {
                eclipseCard.UpdateCardTypeData(game, -1, itemType);
                eclipseCard.UpdateDescription( -1, itemType, null);
                eclipseCard.TokensLayout.UpdateTokenSlots(0);
                if (itemType.TryGetValue(out var timeValueToken, GameJsonKeys.CardDefaultTimerValue))
                {
                    var defaultTimerValue = timeValueToken.GetValue<int>();
                    eclipseCard.TimerLayout.UpdateTimerValue(defaultTimerValue);
                    eclipseCard.TimerLayout.gameObject.SetActive(defaultTimerValue > 0);
                }
                else
                {
                    eclipseCard.TimerLayout.gameObject.SetActive(false);
                }
                eclipseCard.RaycastOff();
                eclipseCard.gameObject.SetActive(true);
            }
        }

        public void ShowSimpleText(JObject tooltipDataObject)
        {
            if (simpleText == null)
                return;
            
            simpleText.text = tooltipDataObject.GetValue<string>(GameJsonKeys.TooltipText);
            simpleText.UpdateFontSize(tooltipDataObject.TryGetValue(GameJsonKeys.TooltipFontSize, out int fontSizeAttribute) ? fontSizeAttribute : 0);
            simpleTextRectTransform.gameObject.SetActive(true);
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
            simpleTextRectTransform.gameObject.SetActive(false);
            eclipseCard.gameObject.SetActive(false);
        }
    }
}