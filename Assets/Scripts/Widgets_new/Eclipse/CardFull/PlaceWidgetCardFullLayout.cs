using Solcery.Games;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Simple;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.CardFull
{
    public class PlaceWidgetCardFullLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        protected RectTransform cardTransform;
        [SerializeField]
        private RectTransform cardFrontTransform;
        [SerializeField]
        private RectTransform cardBackTransform;
        [SerializeField]
        private RectTransform iconFrame;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private RectTransform iconRectTransform;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField] 
        private ClickHandlerBehaviour[] clickHandlers;
        [SerializeField]
        private Texture2D defaultTexture;

        public RectTransform CardBackTransform => cardBackTransform;
        public RectTransform CardFrontTransform => cardFrontTransform;
        public RectTransform CardTransform => cardTransform;
        
        private Sprite _sprite;

        private void Awake()
        {
            if (iconImage.sprite == null)
            {
                UpdateSprite(defaultTexture);
            }
        }

        public virtual void UpdateCardType(IGame game, int objectId, IItemType itemType)
        {
            var displayedName = itemType.TryGetValue(out var valueCardNameToken, GameJsonKeys.CardDisplayedName, objectId) 
                ? valueCardNameToken.GetValue<string>()
                : string.Empty;
            var nameFontSize = itemType.TryGetValue(out var valueNameFontSizeAttributeToken, GameJsonKeys.CardNameFontSizeFull, objectId)
                ? valueNameFontSizeAttributeToken.GetValue<float>()
                : 0f;
            UpdateName(displayedName, nameFontSize);

            var description = itemType.TryGetValue(out var valueCardDescriptionToken, GameJsonKeys.CardDescription, objectId)
                ? valueCardDescriptionToken.GetValue<string>()
                : string.Empty;
                
            var descriptionFontSize = itemType.TryGetValue(out var valueDescriptionFontSizeAttributeToken, GameJsonKeys.CardDescriptionFontSizeFull, objectId)
                    ? valueDescriptionFontSizeAttributeToken.GetValue<float>()
                    : 0f;
            UpdateDescription(description, descriptionFontSize);

            if (itemType.TryGetValue(out var valuePictureToken, GameJsonKeys.Picture, objectId)
                && game.ServiceResource.TryGetTextureForKey(valuePictureToken.GetValue<string>(), out var texture))
            {
                UpdateSprite(texture);
            }
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
        
        private void UpdateName(string newName, float fontSize)
        {
            nameText.text = newName;
            nameText.UpdateFontSize(fontSize);
        }

        private void UpdateDescription(string newDescription, float fontSize)
        {
            descriptionText.text = newDescription;
            descriptionText.UpdateFontSize(fontSize);
        }
        
        public override void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            cardTransform.anchorMin = anchorMin;
            cardTransform.anchorMax = anchorMax;
        }

        private void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            if (texture == null)
            {
                texture = defaultTexture;
            }
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            iconImage.sprite = _sprite;
            UpdateIconSize();
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

        private void OnEnable()
        {
            UpdateIconSize();
        }
        
        private void UpdateIconSize()
        {
            if (iconImage.sprite == null 
                || iconImage.sprite.texture == null)
            {
                return;
            }
            
            var texture = iconImage.sprite.texture;
            var textureSize = new Vector2(texture.width, texture.height);
            var iconSize = iconFrame.rect.size;
            var iconAspect = textureSize.y / textureSize.x;
            var width = iconSize.x;
            var height = iconAspect * width;
            iconRectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}