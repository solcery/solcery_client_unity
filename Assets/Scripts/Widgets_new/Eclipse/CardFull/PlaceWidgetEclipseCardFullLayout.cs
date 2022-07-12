using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Simple;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.CardFull
{
    public class PlaceWidgetEclipseCardFullLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private RectTransform cardTransform;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private TMP_Text typeText;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private EclipseCardTokensLayout tokensLayout;
        [SerializeField]
        private EclipseCardTimerLayout timerLayout;
        [SerializeField] 
        private ClickHandlerBehaviour[] clickHandlers;

        private Sprite _sprite;
        public EclipseCardTokensLayout TokensLayout => tokensLayout;
        public EclipseCardTimerLayout TimerLayout => timerLayout;
        public RectTransform CardTransform => cardTransform;

        public void UpdateCardType(IGame game, EclipseCardTypes type, JObject cardTypeDataObject)
        {
            var typeFontSize = cardTypeDataObject.TryGetValue(GameJsonKeys.CardTypeFontSize, out int typeFontSizeAttribute) ? typeFontSizeAttribute : 20f;
            UpdateType(type.ToString(), typeFontSize);

            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardName, out string name))
            {
                var nameFontSize = cardTypeDataObject.TryGetValue(GameJsonKeys.CardNameFontSize, out int nameFontSizeAttribute) ? nameFontSizeAttribute : 20f;
                UpdateName(name, nameFontSize);
            }

            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardDescription, out string description))
            {
                var descriptionFontSize = cardTypeDataObject.TryGetValue(GameJsonKeys.CardDescriptionFontSize, out int descriptionFontSizeAttribute) ? descriptionFontSizeAttribute : 40f;
                UpdateDescription(description, descriptionFontSize);
            }

            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardPicture, out string picture)
                && game.ServiceResource.TryGetTextureForKey(picture, out var texture))
            {
                UpdateSprite(texture);
            }
                
            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardTimerText, out string timerText))
            {
                TimerLayout.UpdateTimerTextActive(true);
                TimerLayout.UpdateTimerText(timerText);
            }
            else
            {
                TimerLayout.UpdateTimerTextActive(false);
            }        
        }
        
        public override void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            cardTransform.anchorMin = anchorMin;
            cardTransform.anchorMax = anchorMax;
        }

        private void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            iconImage.sprite = _sprite;
        }
        
        private void UpdateName(string newName, float fontSize)
        {
            nameText.text = newName;
            nameText.fontSize = fontSize;
        }

        private void UpdateDescription(string newDescription, float fontSize)
        {
            descriptionText.text = newDescription;
            descriptionText.fontSize = fontSize;
        }
        
        private void UpdateType(string newType, float fontSize)
        {
            typeText.text = newType;
            typeText.fontSize = fontSize;
        }
        
        public void ClearAllOnClickListener()
        {
            foreach (var clickHandler in clickHandlers)
            {
                clickHandler.Cleanup();
            }
        }

        public void AddOnLeftClickListener(UnityAction onLeftClick)
        {
            foreach (var clickHandler in clickHandlers)
            {
                clickHandler.OnLeftClick = onLeftClick;
            }
        }

        public void AddOnRightClickListener(UnityAction onRightClick)
        {
            foreach (var clickHandler in clickHandlers)
            {
                clickHandler.OnRightClick = onRightClick;
            }
        }
        
        private void DestroySprite()
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
        }
        
        private void OnDestroy()
        {
            DestroySprite();
        }
    }
}
