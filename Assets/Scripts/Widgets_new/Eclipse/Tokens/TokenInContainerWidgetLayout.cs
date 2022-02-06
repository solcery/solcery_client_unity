using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public class TokenInContainerWidgetLayout : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image image;
        [SerializeField]
        private TextMeshProUGUI count;
        
        private Sprite _sprite;
        
        public RectTransform RectTransform => rectTransform;

        public void UpdateCount(int value)
        {
            count.text = value.ToString();
        }

        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = _sprite;
        }
        
        public void UpdateParent(Transform parent)
        {
            rectTransform.SetParent(parent, false);
        }

        public void Cleanup()
        {
        }
        
        private void OnDestroy()
        {
            DestroySprite();
        }

        private void DestroySprite()
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
        }
    }
}
