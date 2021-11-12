using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Button
{
    public class WidgetButton : Widget
    {
        private const float AnchorDivider = 10000.0f;
        
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly List<WidgetBehaviourBase> _behaviours;
        private readonly bool _visible;
        private readonly Vector2 _anchorMin;
        private readonly Vector2 _anchorMax;

        // todo delete it
        public GameObject _buttonObject;

        public WidgetButton(IWidgetCanvas widgetCanvas, JObject jsonData)
        {
            _widgetCanvas = widgetCanvas;
            _behaviours = new List<WidgetBehaviourBase>();
            _buttonObject = (GameObject) Resources.Load("ui/button");
            
            if (jsonData.TryGetValue("visible", out var visibleToken))
            {
                _visible = visibleToken.Value<bool>();
            }

            if (jsonData.TryGetValue("x1", out var x1) && jsonData.TryGetValue("y1", out var y1))
            {
                _anchorMin = new Vector2(x1.Value<int>() / AnchorDivider, y1.Value<int>() / AnchorDivider);
            }

            if (jsonData.TryGetValue("x2", out var x2) && jsonData.TryGetValue("y2", out var y2))
            {
                _anchorMax = new Vector2(x2.Value<int>() / AnchorDivider, y2.Value<int>() / AnchorDivider);
            }
        }

        public override void UpdateWidget(EcsWorld world, int[] entityIds)
        {
            Cleanup();
            Init(world, entityIds);
        }

        private void AddWidgetBehaviour(WidgetBehaviourBase behaviour)
        {
            behaviour.ApplyAnchor(_anchorMin, _anchorMax);
           _behaviours.Add(behaviour);
            
        }

        private void Init(EcsWorld world, int[] entityIds)
        {            
            foreach (var entityId in entityIds)
            {
                var behaviour = _widgetCanvas.GetWidgetPool().GetFromPool<WidgetBehaviourButton>(_buttonObject, _widgetCanvas.GetUiCanvas());
                behaviour.Init(world, entityId);
                AddWidgetBehaviour(behaviour);
            }
        }

        private void Cleanup()
        {
            // clear all
        }
    }
}