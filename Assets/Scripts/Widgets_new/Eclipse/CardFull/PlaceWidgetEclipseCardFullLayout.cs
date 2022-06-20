using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Simple;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.CardFull
{
    public class PlaceWidgetEclipseCardFullLayout : PlaceWidgetSimpleLayout
    {
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

        private Sprite _sprite;
        public EclipseCardTokensLayout TokensLayout => tokensLayout;
        
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
