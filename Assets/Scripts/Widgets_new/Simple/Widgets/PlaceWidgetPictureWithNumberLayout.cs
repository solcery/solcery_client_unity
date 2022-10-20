using Solcery.Widgets_new.Eclipse.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Simple.Widgets
{
    public sealed class PlaceWidgetPictureWithNumberLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private TMP_Text text;
        [SerializeField] 
        private Animator animatorDiff;
        [SerializeField] 
        private TextMeshProUGUI textDiff;
        [SerializeField]
        private EclipseCardEffectLayout effectLayout;
        [SerializeField]
        private RectTransform widgetContentTransform;
        [SerializeField]
        private RectTransform imageTransform;

        public RectTransform WidgetContentTransform => widgetContentTransform;
        public RectTransform ImageTransform => imageTransform;
        public EclipseCardEffectLayout EffectLayout => effectLayout;

        private Sprite _sprite;

        public void UpdateImage(Texture2D texture)
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = _sprite;
        }
        
        public void UpdateNumber(int value)
        {
            text.text = value.ToString();
        }
        
        public void ShowDiff(int diff)
        {
            var diffStr = diff.ToString();
            textDiff.text = diff > 0 ? $"+ {diffStr}" : $"- {diffStr}";
            animatorDiff.SetTrigger(diff > 0 ? "Increased" : "Decreased");
        }
    }
}