using System;
using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardFull
{
    public class PlaceWidgetEclipseCardFullLayout : PlaceWidgetCardFullLayout
    {
        [SerializeField]
        private TMP_Text typeText;
        [SerializeField]
        private EclipseCardTokensLayout tokensLayout;
        [SerializeField]
        private EclipseCardTimerLayout timerLayout;

        public EclipseCardTokensLayout TokensLayout => tokensLayout;
        public EclipseCardTimerLayout TimerLayout => timerLayout;
        public RectTransform CardTransform => cardTransform;

        public override void UpdateCardType(IGame game, int objectId, IItemType itemType)
        {
            base.UpdateCardType(game, objectId, itemType);

            var displayedType =
                itemType.TryGetValue(out var valueDisplayedTypeToken, GameJsonKeys.CardDisplayedType, objectId)
                    ? valueDisplayedTypeToken.GetValue<string>()
                    : string.Empty;
            var typeFontSize =
                itemType.TryGetValue(out var valueTypeFontSizeAttributeToken, GameJsonKeys.CardTypeFontSize, objectId)
                    ? valueTypeFontSizeAttributeToken.GetValue<int>()
                    : 20f;
            UpdateType(displayedType, typeFontSize);

            if (itemType.TryGetValue(out var valueTimerTextToken, GameJsonKeys.CardTimerText, objectId))
            {
                TimerLayout.UpdateTimerTextActive(true);
                TimerLayout.UpdateTimerText(valueTimerTextToken.GetValue<string>());
            }
            else
            {
                TimerLayout.UpdateTimerTextActive(false);
            }        
        }
        private void UpdateType(string displayedType, float fontSize)
        {
            typeText.text = displayedType;
            typeText.fontSize = fontSize;
        }
    }
}
