using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Simple.Pictures
{
    public sealed class PlaceWidgetPictureLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private Image picture;
        
        private Sprite _sprite;

        public void UpdatePicture(Texture2D texture)
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            picture.sprite = _sprite;
        }
    }
}