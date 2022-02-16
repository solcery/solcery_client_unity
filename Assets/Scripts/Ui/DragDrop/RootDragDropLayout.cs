using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Ui.DragDrop
{
    public sealed class RootDragDropLayout : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler
    {
        [SerializeField]
        private RectTransform rectTransform;
        
        private enum DragDropStates
        {
            Free,
            Drag
        }

        private DragDropStates _currentDragDropState = DragDropStates.Free;
        private int _currentDraggableObjectId;

        public void UpdateOnDrag(int draggableObjectId)
        {
            _currentDragDropState = DragDropStates.Drag;
            _currentDraggableObjectId = draggableObjectId;
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
                {"entity_id", new JValue(_currentDraggableObjectId)},
                {"world_position", position.ToJObject()}
            };

            ServiceEvents.Current.BroadcastEvent(UiEvents.UiDropEvent, ed);
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
                {"entity_id", new JValue(_currentDraggableObjectId)},
                {"world_position", position.ToJObject()}
            };
            
            ServiceEvents.Current.BroadcastEvent(UiEvents.UiDragMoveEvent, ed);
        }
    }
}