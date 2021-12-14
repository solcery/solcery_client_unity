using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField]
        private Animator animator;
        
        private Sprite _sprite;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _pivot;
        private Vector2 _anchoredPosition;
        private Vector2 _offsetMax;
        private Vector2 _offsetMin;

        private void Awake()
        {
            _anchoredPosition = rectTransform.anchoredPosition;
            _anchorMin = rectTransform.anchorMin;
            _anchorMax = rectTransform.anchorMax;
            _pivot = rectTransform.pivot;
            _offsetMin = rectTransform.offsetMin;
            _offsetMax = rectTransform.offsetMax;
        }

        public void UpdateParent(Transform parent)
        {
            rectTransform.SetParent(parent, false);
            rectTransform.anchoredPosition = _anchoredPosition;
            rectTransform.offsetMax = Vector2.down;
            rectTransform.pivot = _pivot;
            rectTransform.anchorMin = _anchorMin;
            rectTransform.anchorMax = _anchorMax;
            rectTransform.offsetMin = _offsetMin;
            rectTransform.offsetMax = _offsetMax;
        }

        public void Move(Vector2 oldPosition)
        {
            var newPosition = rectTransform.position;
            rectTransform.position = oldPosition;
            DOTween.Sequence()
                .Append(transform.DOMove(newPosition, 1f))
                .Play();
        }

        public void UpdateCardFace(PlaceWidgetCardFace cardFace, bool withAnimation)
        {
            if (withAnimation)
            {
                animator.enabled = true;
                if (cardFace == PlaceWidgetCardFace.Down)
                {
                    animator.SetTrigger("TurnFaceDown");
                }

                if (cardFace == PlaceWidgetCardFace.Up)
                {
                    animator.SetTrigger("TurnFaceUp");
                }
            }
            else
            {
                animator.enabled = false;
                back.SetActive(cardFace == PlaceWidgetCardFace.Down);
            }
        }

        public void UpdateInteractable(bool interactable)
        {
            button.interactable = interactable;
        }

        public void AddOnClickListener(UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
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
            button.onClick.RemoveAllListeners();
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