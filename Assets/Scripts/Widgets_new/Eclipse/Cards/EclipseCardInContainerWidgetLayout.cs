using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.Cards.Traits;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public sealed class EclipseCardInContainerWidgetLayout : MonoBehaviour, IPointerClickHandler
    {
        public RectTransform RectTransform => rectTransform;
        [HideInInspector]
        public int EntityId;
        
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private EclipseCardTimerLayout timerLayout;
        [SerializeField]
        private EclipseCardTraitsLayout traitsLayout;
        [SerializeField]
        private EclipseCardTokensLayout tokensLayout;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text descriptionText;
        [SerializeField]
        private List<Graphic> raycastObjects;

        private Sprite _sprite;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _pivot;
        private Vector2 _anchoredPosition;
        private Vector2 _offsetMax;
        private Vector2 _offsetMin;
        
        private Transform _dragDropTransform;
        private readonly Dictionary<Graphic, bool> _raycastTargetSettings = new();

        private void Awake()
        {
            _anchoredPosition = rectTransform.anchoredPosition;
            _anchorMin = rectTransform.anchorMin;
            _anchorMax = rectTransform.anchorMax;
            _pivot = rectTransform.pivot;
            _offsetMin = rectTransform.offsetMin;
            _offsetMax = rectTransform.offsetMax;
        }

        public void SetDragDropTransform(Transform dragDropTransform)
        {
            _dragDropTransform = dragDropTransform;
        }

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
            rectTransform.rotation = Quaternion.identity;
        }
        
        public void UpdateSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }
        
        public void Cleanup()
        {
        }
        
        private void OnDestroy()
        {
            DestroySprite();
        }

        public void UpdateName(string newName)
        {
            nameText.text = newName;
        }

        public void UpdateDescription(string newDescription)
        {
            descriptionText.text = newDescription;
        }

        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            iconImage.sprite = _sprite;
        }
        
        private void DestroySprite()
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
        }

        public void UpdateTimer(bool isShow, int timer)
        {
            timerLayout.UpdateTimer(isShow, timer);
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
            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );

            var ed = new JObject
            {
                {"entity_id", new JValue(EntityId)},
                {"world_position", position.ToJObject()}
            };
            
            ServiceEvents.Current.BroadcastEvent(UiEvents.UiDragEvent, ed);
        }
    }
}