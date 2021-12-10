using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public sealed class CardInContainerWidgetLayout : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private GameObject back;
        [SerializeField]
        private TMP_Text cardName;
        [SerializeField]
        private TMP_Text description;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image image;
        
        private Sprite _sprite;

        public void UpdateParent(Transform parent)
        {
            rectTransform.SetParent(parent, false);
        }

        public void UpdateCardFace(PlaceWidgetCardFace cardFace)
        {
            back.SetActive(cardFace == PlaceWidgetCardFace.Down);
        }

        public void UpdateInteractable(bool interactable)
        {
            button.interactable = interactable;
        }

        public void UpdateName(string newName)
        {
            cardName.text = newName;
        }

        public void UpdateDescription(string newDescription)
        {
            description.text = newDescription;
        }

        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = _sprite;
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