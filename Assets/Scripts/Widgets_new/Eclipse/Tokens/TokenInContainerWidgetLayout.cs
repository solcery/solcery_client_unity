using System.Collections.Generic;
using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public class TokenInContainerWidgetLayout : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector]
        public int EntityId;
        public PlaceWidget ParentPlaceWidget;
        
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image image;
        [SerializeField]
        private List<Graphic> raycastObjects;
        
        private Sprite _sprite;
        private readonly Dictionary<Graphic, bool> _raycastTargetSettings = new Dictionary<Graphic, bool>();
        
        public RectTransform RectTransform => rectTransform;
        public Image Icon => image;
        
        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = _sprite;
        }
        
        public void UpdateParent(Transform parent, bool isDragDrop = false)
        {
            rectTransform.SetParent(parent, false);
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

        public void OnPointerClick(PointerEventData eventData)
        {
            ParentPlaceWidget = transform.parent.GetComponentInParent<PlaceWidgetLayout>().PlaceWidget;
            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );
            ServiceEvents.Current.BroadcastEvent(OnDragEventData.Create(EntityId, position, eventData));
        }
    }
}
