// ----------------------------------------------------------------------------
// The MIT License
// Ugui bindings https://github.com/Leopotam/ecslite-unity-ugui
// for LeoECS Lite https://github.com/Leopotam/ecslite
// Copyright (c) 2021 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine.EventSystems;

namespace Leopotam.EcsLite.Unity.Ugui {
    public sealed class EcsUguiDragEndAction : EcsUguiActionBase<EcsUguiDragEndEvent>, IEndDragHandler {
        public void OnEndDrag (PointerEventData eventData) {
            if (IsValidForEvent ()) {
                ref var msg = ref CreateEvent ();
                msg.WidgetName = GetWidgetName ();
                msg.Sender = gameObject;
                msg.Position = eventData.position;
                msg.PointerId = eventData.pointerId;
                msg.Button = eventData.button;
            }
        }
    }
}