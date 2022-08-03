using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
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
        private RectTransform iconFrame;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private RectTransform iconRectTransform;
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
        
        private Texture2D _newTexture;
        private Vector2 _previewIconFrameSize;
        private Vector2 _textureSize;

        private void Awake()
        {
            _previewIconFrameSize = iconFrame.rect.size;
            _textureSize = _previewIconFrameSize;
        }

        public void UpdateCardType(IGame game, EclipseCardTypes type, int objectId, IItemType itemType)
        {
            var typeFontSize =
                itemType.TryGetValue(out var valueTypeFontSizeAttributeToken, GameJsonKeys.CardTypeFontSize, objectId)
                    ? valueTypeFontSizeAttributeToken.GetValue<int>()
                    : 20f;
            UpdateType(type.ToString(), typeFontSize);

            if (itemType.TryGetValue(out var valueCardNameToken, GameJsonKeys.CardName, objectId))
            {
                var nameFontSize =
                    itemType.TryGetValue(out var valueNameFontSizeAttributeToken, GameJsonKeys.CardNameFontSize,
                        objectId)
                        ? valueNameFontSizeAttributeToken.GetValue<int>()
                        : 20f;
                UpdateName(valueCardNameToken.GetValue<string>(), nameFontSize);
            }

            if (itemType.TryGetValue(out var valueCardDescriptionToken, GameJsonKeys.CardDescription, objectId))
            {
                var descriptionFontSize =
                    itemType.TryGetValue(out var valueDescriptionFontSizeAttributeToken,
                        GameJsonKeys.CardDescriptionFontSizeFull, objectId)
                        ? valueDescriptionFontSizeAttributeToken.GetValue<int>()
                        : 40f;
                UpdateDescription(valueCardDescriptionToken.GetValue<string>(), descriptionFontSize);
            }

            if (itemType.TryGetValue(out var valuePictureToken, GameJsonKeys.CardPicture, objectId)
                && game.ServiceResource.TryGetTextureForKey(valuePictureToken.GetValue<string>(), out var texture))
            {
                UpdateSprite(texture);
            }
                
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
        
        public override void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            cardTransform.anchorMin = anchorMin;
            cardTransform.anchorMax = anchorMax;
        }

        private void UpdateSprite(Texture2D texture)
        {
            _newTexture = texture;
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
        
        private void LateUpdate()
        {
            if (_newTexture == null && iconFrame.rect.size != _previewIconFrameSize)
            {
                return;
            }

            // Update sprite
            if (_newTexture != null)
            {
                DestroySprite();
                _sprite = Sprite.Create(_newTexture, new Rect(0.0f, 0.0f, _newTexture.width, _newTexture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);
                iconImage.sprite = _sprite;
                _textureSize = new Vector2(_newTexture.width, _newTexture.height);
                _newTexture = null;
            }
            
            // Update size
            _previewIconFrameSize = iconFrame.rect.size;
            var iconAspect = _textureSize.y / _textureSize.x;
            var width = _previewIconFrameSize.x;
            var height = iconAspect * width;
            iconRectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}
