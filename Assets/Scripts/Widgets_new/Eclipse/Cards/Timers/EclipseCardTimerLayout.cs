using Solcery.Games;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Cards.Timers
{
    public sealed class EclipseCardTimerLayout : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timerValue;
        [SerializeField]
        private TMP_Text timerText;
        [SerializeField]
        private Image[] filings;
        
        public void UpdateTimerValue(int value)
        {
            timerValue.text = value.ToString();
        }
        
        public void UpdateTypeData(int objectId, IItemType itemType)
        {
            if (itemType.TryGetValue(out var valueTimerTextToken, GameJsonKeys.CardTimerText, objectId))
            {
                UpdateTimerTextActive(true);
                UpdateTimerText(valueTimerTextToken.GetValue<string>());
            }
            else
            {
                UpdateTimerTextActive(false);
            }   
            
            if (itemType.TryGetValue(out var valueTimerFilingToken, GameJsonKeys.CardDurationFiling, objectId))
            {
                UpdateFillColor(valueTimerFilingToken.GetValue<string>());
            }
        }

        private void UpdateTimerText(string text)
        {
            timerText.text = text;
        }
        
        private void UpdateTimerTextActive(bool active)
        {
            timerText.gameObject.SetActive(active);
        }

        private void UpdateFillColor(string fillColor)
        {
            if (fillColor != null)
            {
                if (ColorUtility.TryParseHtmlString(fillColor, out var color))
                {
                    foreach (var image in filings)
                    {
                        image.color = color;
                    }
                }
            }
        }


        public void Cleanup()
        {
        }
    }
}