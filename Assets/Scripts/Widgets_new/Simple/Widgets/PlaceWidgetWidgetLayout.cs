using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Simple.Widgets
{
    public sealed class PlaceWidgetWidgetLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private TMP_Text text;

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
    }
}