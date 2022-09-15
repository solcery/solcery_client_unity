using System.Collections.Generic;
using DG.Tweening;
using Solcery.Services.Events;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Nft.Card
{
    public class EclipseCardNftInContainerWidgetLayout : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        public RectTransform RectTransform => rectTransform;
        public RectTransform FrontTransform => frontTransform;
        public RectTransform BackTransform => backTransform;
        
        [HideInInspector]
        public int EntityId;
        public int? AttachEntityId = null;
        public PlaceWidget ParentPlaceWidget;
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private RectTransform frontTransform;
        [SerializeField]
        private RectTransform backTransform;
        [SerializeField]
        private RectTransform iconFrame;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private RectTransform iconRectTransform;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text descriptionText;
        [SerializeField]
        private List<Graphic> raycastObjects;
        [SerializeField]
        private Texture2D defaultTexture;
        
        private readonly Dictionary<Graphic, bool> _raycastTargetSettings = new Dictionary<Graphic, bool>();
        
        private Sprite _sprite;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _pivot;
        private Vector2 _anchoredPosition;
        private Vector2 _offsetMax;
        private Vector2 _offsetMin;
        private CustomImages _images;
        
        private void Awake()
        {
            _anchoredPosition = rectTransform.anchoredPosition;
            _anchorMin = rectTransform.anchorMin;
            _anchorMax = rectTransform.anchorMax;
            _pivot = rectTransform.pivot;
            _offsetMin = rectTransform.offsetMin;
            _offsetMax = rectTransform.offsetMax;
            if (iconImage.sprite == null)
            {
                UpdateSprite(defaultTexture);
            }
            _images = gameObject.AddComponent<CustomImages>();
        }
        
        private void OnEnable()
        {
            UpdateIconSize();
        }
        
        public void UpdateName(string newName, float fontSize)
        {
            nameText.text = newName;
            nameText.UpdateFontSize(fontSize);
        }

        public void UpdateDescription(string newDescription, float fontSize)
        {
            descriptionText.text = newDescription;
            descriptionText.UpdateFontSize(fontSize);
        }
        
        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            if (texture == null)
            {
                texture = defaultTexture;
            }
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            iconImage.sprite = _sprite;
            UpdateIconSize();
        }
        
        public void SetActive(bool active)
        {
            FrontTransform.gameObject.SetActive(active);

            if (active)
            {
                RaycastOn();
            }
            else
            {
                RaycastOff();
            }
        }
        
        public void RaycastOn()
        {
            foreach (var targetSetting in _raycastTargetSettings)
            {
                targetSetting.Key.raycastTarget = targetSetting.Value;
            }
        }

        public void RaycastOff()
        {
            _raycastTargetSettings.Clear();
            foreach (var raycastObject in raycastObjects)
            {
                _raycastTargetSettings.Add(raycastObject, raycastObject.raycastTarget);
                raycastObject.raycastTarget = false;
            }
        }

        private Tweener _tween;
        private bool _isClick;
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _isClick = true;
            _tween = DOTween.To(value => { }, 0, 1, .5f);
            _tween.OnComplete(OnRightClick);
        }

        private void OnRightClick()
        {
            _tween?.OnComplete(null);
            _tween = null;
            _isClick = false;
            OnOnPointerRightButtonClick();
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            var comp = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<EclipseCardNftInContainerWidgetLayout>();
            if (comp != this)
            {
                _isClick = false;
                _tween?.Kill();
                _tween = null;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = null;

            if (_isClick)
            {
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        OnOnPointerLeftButtonClick(eventData);
                        break;
                    case PointerEventData.InputButton.Right:
                        OnOnPointerRightButtonClick();
                        break;
                }
            }
        }
        
        private void OnOnPointerLeftButtonClick(PointerEventData eventData)
        {
            if (AttachEntityId == null)
            {
                return;
            }
            
            ParentPlaceWidget = transform.parent.GetComponentInParent<PlaceWidgetLayout>().PlaceWidget;
            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );
            ServiceEvents.Current.BroadcastEvent(OnDragEventData.Create(EntityId, AttachEntityId.Value, position, eventData));
        }
        
        private void OnOnPointerRightButtonClick()
        {
            ServiceEvents.Current.BroadcastEvent(OnRightClickEventData.Create(EntityId));
        }
        
        public void Cleanup() { }
        
        public void UpdateParent(Transform parent, bool isDragDrop = false)
        {
            rectTransform.SetParent(parent, false);
            if (!isDragDrop)
            {
                rectTransform.anchoredPosition = _anchoredPosition;
                rectTransform.offsetMax = Vector2.down;
                rectTransform.pivot = _pivot;
                rectTransform.anchorMin = _anchorMin;
                rectTransform.anchorMax = _anchorMax;
                rectTransform.offsetMin = _offsetMin;
                rectTransform.offsetMax = _offsetMax;
            }

            rectTransform.localScale = Vector3.one;
        }
        
        public void UpdateSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }
        
        private void DestroySprite()
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
        }
        
        private void UpdateIconSize()
        {
            if (iconImage.sprite == null 
                || iconImage.sprite.texture == null)
            {
                return;
            }
            
            var texture = iconImage.sprite.texture;
            var textureSize = new Vector2(texture.width, texture.height);
            var iconSize = iconFrame.rect.size;
            var iconAspect = textureSize.y / textureSize.x;
            var width = iconSize.x;
            var height = iconAspect * width;
            iconRectTransform.sizeDelta = new Vector2(width, height);
        }
        
        public void UpdateAvailable(bool available)
        {
            _images.UpdateAvailable(available);
        }
    }
}