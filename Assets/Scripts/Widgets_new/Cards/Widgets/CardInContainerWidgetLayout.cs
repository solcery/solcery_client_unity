using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public sealed class CardInContainerWidgetLayout : MonoBehaviour
    {
        public Vector3 WorldPosition => rectTransform.position;
        
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
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        
        private static readonly int TurnFaceDown = Animator.StringToHash("TurnFaceDown");
        private static readonly int TurnFaceUp = Animator.StringToHash("TurnFaceUp");

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

        public void UpdateSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }

        public void Move(Vector3 from, Vector3 to, Action onMoveComplete)
        {
            rectTransform.position = from;

            KillMoveTween();
            
            _moveTween = rectTransform.DOMove(to, 1f).OnComplete(() =>
            {
                onMoveComplete.Invoke();
                _moveTween = null;
            }).Play();
        }

        public void UpdateCardFace(PlaceWidgetCardFace cardFace, bool withAnimation)
        {
            if (withAnimation)
            {
                animator.enabled = true;
                if (cardFace == PlaceWidgetCardFace.Down)
                {
                    animator.SetTrigger(TurnFaceDown);
                }

                if (cardFace == PlaceWidgetCardFace.Up)
                {
                    animator.SetTrigger(TurnFaceUp);
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
            KillMoveTween();
            button.onClick.RemoveAllListeners();
        }

        private void KillMoveTween()
        {
            if (_moveTween != null)
            {
                _moveTween.Kill();
                _moveTween = null;
            }
        }
        
        private void OnDestroy()
        {
            KillMoveTween();
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