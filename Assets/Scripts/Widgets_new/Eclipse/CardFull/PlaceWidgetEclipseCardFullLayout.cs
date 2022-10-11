using DG.Tweening;
using Solcery.Games;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.Effects;
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
        [SerializeField]
        private EclipseCardEffectLayout effectLayout;
        [SerializeField]
        private GameObject[] highlights;

        public EclipseCardTokensLayout TokensLayout => tokensLayout;
        public EclipseCardTimerLayout TimerLayout => timerLayout;
        public EclipseCardEffectLayout EffectLayout => effectLayout;
        
        private Sequence _sequence;
        private PlaceWidgetCardFace _cardFace;

        public override void UpdateCardType(IGame game, int objectId, IItemType itemType)
        {
            base.UpdateCardType(game, objectId, itemType);

            var displayedType = itemType.TryGetValue(out var valueDisplayedTypeToken, GameJsonKeys.CardDisplayedType, objectId)
                    ? valueDisplayedTypeToken.GetValue<string>()
                    : string.Empty;
            var typeFontSize = itemType.TryGetValue(out var valueTypeFontSizeAttributeToken, GameJsonKeys.CardTypeFontSizeFull, objectId)
                    ? valueTypeFontSizeAttributeToken.GetValue<float>()
                    : 0f;
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
            typeText.UpdateFontSize(fontSize);
        }
        
        public void UpdateHighlight(bool active)
        {
            foreach (var highlight in highlights)
            {
                highlight.SetActive(active);
            }
        }
        
        public void UpdateCardFace(PlaceWidgetCardFace cardFace, bool withAnimation)
        {
            if (withAnimation)
            {
                PlayCardTurn(cardFace);
            }
            else
            {
                UpdateView(cardFace);
            }
        }
        
        private void PlayCardTurn(PlaceWidgetCardFace cardFace)
        {
            _sequence?.Complete();
            _sequence = DOTween.Sequence()
                .Append(transform.DORotate(new Vector3(0, 90, 0), 0.3f))
                .AppendCallback(() =>
                {
                    UpdateView(cardFace);
                })
                .Append(transform.DORotate(new Vector3(0, 0, 0), 0.3f))
                .Play();
        }
        
        private void UpdateView(PlaceWidgetCardFace cardFace)
        {
            _cardFace = cardFace;
            CardFrontTransform.gameObject.SetActive(_cardFace != PlaceWidgetCardFace.Down);
            CardBackTransform.gameObject.SetActive(_cardFace == PlaceWidgetCardFace.Down);
        }
    }
}
