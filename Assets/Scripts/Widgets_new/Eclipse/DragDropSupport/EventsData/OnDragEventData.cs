using Solcery.Services.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData
{
    public sealed class OnDragEventData : EventData
    {
        public const string OnDragEventName = "UI_DRAG_EVENT";

        public int DragObjectEntityId => _dragObjectEntityId;
        public int DragEntityId => _dragEntityId;
        public Vector3 WorldPosition => _worldPosition;
        public PointerEventData PointerEventData => _pointerEventData;

        private readonly int _dragObjectEntityId;
        private readonly int _dragEntityId;
        private readonly Vector3 _worldPosition;
        private readonly PointerEventData _pointerEventData;
        
        public static OnDragEventData Create(int dragObjectEntityId, int dragEntityId, Vector3 worldPosition, PointerEventData pointerEventData)
        {
            return new OnDragEventData(dragObjectEntityId, dragEntityId, worldPosition, pointerEventData);
        }
        
        private OnDragEventData(int dragObjectEntityId, int dragEntityId, Vector3 worldPosition, PointerEventData pointerEventData) : base(OnDragEventName)
        {
            _dragObjectEntityId = dragObjectEntityId;
            _dragEntityId = dragEntityId;
            _worldPosition = worldPosition;
            _pointerEventData = pointerEventData;
        }
    }
}