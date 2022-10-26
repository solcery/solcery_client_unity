using System.Collections.Generic;
using DG.Tweening;
using Solcery.Games;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.CardFull
{
    public class PlaceWidgetEclipseCardFullLayout : PlaceWidgetCardFullLayout, IWidgetLayoutHighlight
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
        private Image[] highlights;

        public EclipseCardTokensLayout TokensLayout => tokensLayout;
        public EclipseCardTimerLayout TimerLayout => timerLayout;
        public EclipseCardEffectLayout EffectLayout => effectLayout;
        public Image[] Highlights => highlights;

        private Sequence _sequence;
        private PlaceWidgetCardFace _cardFace;

        public override void UpdateCardTypeData(IGame game, int objectId, IItemType itemType)
        {
            base.UpdateCardTypeData(game, objectId, itemType);

            var displayedType = itemType.TryGetValue(out var valueDisplayedTypeToken, GameJsonKeys.CardDisplayedType, objectId)
                    ? valueDisplayedTypeToken.GetValue<string>()
                    : string.Empty;
            var typeFontSize = itemType.TryGetValue(out var valueTypeFontSizeAttributeToken, GameJsonKeys.CardTypeFontSizeFull, objectId)
                    ? valueTypeFontSizeAttributeToken.GetValue<float>()
                    : 0f;
            UpdateType(displayedType, typeFontSize);
            TimerLayout.UpdateTypeData(objectId, itemType);
        }
        
        private void UpdateType(string displayedType, float fontSize)
        {
            typeText.text = displayedType;
            typeText.UpdateFontSize(fontSize);
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
