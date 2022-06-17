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
        private TextMeshProUGUI textDiff = null;

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
        
        public void UpdateText(string newText)
        {
            text.text = newText;
        }
        
        public void ShowDiff(int diff)
        {
            var diffStr = diff.ToString();
            textDiff.text = diff > 0 ? $"+ {diffStr}" : $"- {diffStr}";
            animatorDiff.SetTrigger(diff > 0 ? "Increased" : "Decreased");
        }
    }
}