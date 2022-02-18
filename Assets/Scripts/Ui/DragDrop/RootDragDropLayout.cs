using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solcery.Ui.DragDrop
{
    public sealed class RootDragDropLayout : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image image;

        public RectTransform GetRectTransform => rectTransform;
        
        private enum DragDropStates
        {
            Free,
            Drag
        }

        private DragDropStates _currentDragDropState = DragDropStates.Free;
        private int _currentDraggableEntityId;

        public void UpdateOnDrag(int draggableEntityId)
        {
            _currentDragDropState = DragDropStates.Drag;
            _currentDraggableEntityId = draggableEntityId;
            image.enabled = true;
        }
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (_currentDragDropState != DragDropStates.Drag)
            {
                return;
            }

            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, Camera.current,
                out var position);

            var ed = new JObject
            {
                {"event", new JValue(UiEvents.UiDropEvent)},
                {"entity_id", new JValue(_currentDraggableEntityId)},
                {"world_position", position.ToJObject()}
            };

            ServiceEvents.Current.BroadcastEvent(UiEvents.UiDropEvent, ed);

            _currentDragDropState = DragDropStates.Free;
            image.enabled = false;
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            if (_currentDragDropState != DragDropStates.Drag)
            {
                return;
            }
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, Camera.current,
                out var position);

            var ed = new JObject
            {
                {"event", new JValue(UiEvents.UiDragMoveEvent)},
                {"entity_id", new JValue(_currentDraggableEntityId)},
                {"world_position", position.ToJObject()}
            };
            
            ServiceEvents.Current.BroadcastEvent(UiEvents.UiDragMoveEvent, ed);
        }
    }
}