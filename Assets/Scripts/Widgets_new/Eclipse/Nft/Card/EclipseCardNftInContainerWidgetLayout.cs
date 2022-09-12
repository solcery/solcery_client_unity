using System.Collections.Generic;
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
    public class EclipseCardNftInContainerWidgetLayout : MonoBehaviour, IPointerClickHandler
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
        
        private void Awake()
        {
            _anchoredPosition = rectTransform.anchoredPosition;
            _anchorMin = rectTransform.anchorMin;
            _anchorMax = rectTransform.anchorMax;
            _pivot = rectTransform.pivot;
            _offsetMin = rectTransform.offsetMin;
            _offsetMax = rectTransform.offsetMax;

            if (iconImage == null)
            {
                UpdateSprite(defaultTexture);
            }
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

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    OnPointerLeftButtonClick(eventData);
                    break;
                case PointerEventData.InputButton.Right:
                    OnPointerRightButtonClick();
                    break;
            }
        }
        
        private void OnPointerLeftButtonClick(PointerEventData eventData)
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
        
        private void OnPointerRightButtonClick()
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
    }
}