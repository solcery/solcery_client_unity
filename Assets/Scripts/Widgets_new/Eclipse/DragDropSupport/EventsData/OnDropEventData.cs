using Solcery.Services.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData
{
    public sealed class OnDropEventData : EventData
    {
        public const string OnDropEventName = "UI_DROP_EVENT";
        
        public int DragEntityId => _dragEntityId;
        public int DragDropEntityId => _dragDropEntityId;
        public Vector3 WorldPosition => _worldPosition;
        public PointerEventData PointerEventData => _pointerEventData;
        
        private readonly int _dragEntityId;
        private readonly int _dragDropEntityId;
        private readonly Vector3 _worldPosition;
        private readonly PointerEventData _pointerEventData;

        public static OnDropEventData Create(int dragEntityId, int dragDropEntityId, Vector3 worldPosition, PointerEventData pointerEventData)
        {
            return new OnDropEventData(dragEntityId, dragDropEntityId, worldPosition, pointerEventData);
        }

        private OnDropEventData(int dragEntityId, int dragDropEntityId, Vector3 worldPosition, PointerEventData pointerEventData) : base(OnDropEventName)
        {
            _dragEntityId = dragEntityId;
            _dragDropEntityId = dragDropEntityId;
            _worldPosition = worldPosition;
            _pointerEventData = pointerEventData;
        }
    }
}