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

        public override void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            cardTransform.anchorMin = anchorMin;
            cardTransform.anchorMax = anchorMax;
        }

        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            iconImage.sprite = _sprite;
        }
        
        public void UpdateName(string newName)
        {
            nameText.text = newName;
        }

        public void UpdateDescription(string newDescription)
        {
            descriptionText.text = newDescription;
        }
        
        public void UpdateType(string newType)
        {
            typeText.text = newType;
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
