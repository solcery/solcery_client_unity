using Solcery.Services.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Eclipse.Cards.EventsData
{
    public class OnUniversalEventData : EventData
    {
        public const string OnUniversalEventDataName = "UI_UNIVERSAL_EVENT";
        
        public int DragObjectEntityId => _dragObjectEntityId;
        public int DragEntityId => _dragEntityId;
        public Vector3 WorldPosition => _worldPosition;
        public PointerEventData PointerEventData => _pointerEventData;

        private readonly int _dragObjectEntityId;
        private readonly int _dragEntityId;
        private readonly Vector3 _worldPosition;
        private readonly PointerEventData _pointerEventData;
        
        public static OnUniversalEventData Create(int dragObjectEntityId, int dragEntityId, Vector3 worldPosition, PointerEventData pointerEventData)
        {
            return new OnUniversalEventData(dragObjectEntityId, dragEntityId, worldPosition, pointerEventData);
        }

        private OnUniversalEventData(int dragObjectEntityId, int dragEntityId, Vector3 worldPosition, PointerEventData pointerEventData) : base(OnUniversalEventDataName)
        {
            _dragObjectEntityId = dragObjectEntityId;
            _dragEntityId = dragEntityId;
            _worldPosition = worldPosition;
            _pointerEventData = pointerEventData;
        }
    }
}