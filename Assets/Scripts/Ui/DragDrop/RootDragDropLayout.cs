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
        private int _dragObjectEntityId;
        private int _currentDraggableEntityId;
        private int _currentDragDropEntityId;

        public void UpdateOnDrag(int dragObjectEntityId, int draggableEntityId, int dragDropEntityId)
        {
            _currentDragDropState = DragDropStates.Drag;
            _dragObjectEntityId = dragObjectEntityId;
            _currentDraggableEntityId = draggableEntityId;
            _currentDragDropEntityId = dragDropEntityId;
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

            ServiceEvents.Current.BroadcastEvent(OnDropEventData.Create(_dragObjectEntityId, _currentDraggableEntityId, _currentDragDropEntityId, position, eventData));

            _currentDragDropState = DragDropStates.Free;
            _dragObjectEntityId = -1;
            _currentDraggableEntityId = -1;
            _currentDragDropEntityId = -1;
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
            
            //Debug.Log($"Event pos {eventData.position} world pos {position}");
            
            ServiceEvents.Current.BroadcastEvent(OnDragMoveEventData.Create(_currentDraggableEntityId, position, eventData));
        }
    }
}