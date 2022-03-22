using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;
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

            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );

            ServiceEvents.Current.BroadcastEvent(OnDropEventData.Create(_currentDraggableEntityId, position, eventData));

            _currentDragDropState = DragDropStates.Free;
            image.enabled = false;
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            if (_currentDragDropState != DragDropStates.Drag)
            {
                return;
            }
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );
            
            Debug.Log($"Event pos {eventData.position} world pos {position}");
            
            ServiceEvents.Current.BroadcastEvent(OnDragMoveEventData.Create(_currentDraggableEntityId, position, eventData));
        }
    }
}