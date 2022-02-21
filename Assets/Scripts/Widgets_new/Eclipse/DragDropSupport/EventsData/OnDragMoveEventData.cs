using Solcery.Services.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData
{
    public sealed class OnDragMoveEventData : EventData
    {
        public const string OnDragMoveEventName = "UI_DRAG_MOVE_EVENT";
        
        public int DragEntityId => _dragEntityId;
        public Vector3 WorldPosition => _worldPosition;
        public PointerEventData PointerEventData => _pointerEventData;
        
        private readonly int _dragEntityId;
        private readonly Vector3 _worldPosition;
        private readonly PointerEventData _pointerEventData;
        
        public static OnDragMoveEventData Create(int dragEntityId, Vector3 worldPosition, PointerEventData pointerEventData)
        {
            return new OnDragMoveEventData(dragEntityId, worldPosition, pointerEventData);
        }

        private OnDragMoveEventData(int dragEntityId, Vector3 worldPosition, PointerEventData pointerEventData) : base(OnDragMoveEventName)
        {
            _dragEntityId = dragEntityId;
            _worldPosition = worldPosition;
            _pointerEventData = pointerEventData;
        }
    }
}