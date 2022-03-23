// ----------------------------------------------------------------------------
// The MIT License
// Ugui bindings https://github.com/Leopotam/ecslite-unity-ugui
// for LeoECS Lite https://github.com/Leopotam/ecslite
// Copyright (c) 2021 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace Leopotam.EcsLite.Unity.Ugui {
    public struct EcsUguiClickEvent {
        public string WidgetName;
        public GameObject Sender;
        public Vector2 Position;
        public PointerEventData.InputButton Button;
    }
}