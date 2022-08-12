using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
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
        
        private Sprite _sprite;

        public virtual void UpdateCardType(IGame game, EclipseCardTypes type, int objectId, IItemType itemType)
        {
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
            nameText.fontSize = fontSize;
        }

        private void UpdateDescription(string newDescription, float fontSize)
        {
            descriptionText.text = newDescription;
            descriptionText.fontSize = fontSize;
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