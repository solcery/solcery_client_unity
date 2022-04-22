using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.States.Games.Attrs;
using Solcery.DebugViewers.States.Games.Objects;
using Solcery.DebugViewers.Views.Attrs;
using Solcery.DebugViewers.Views.Objects;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.DebugViewers.States.Games
{
    public sealed class DebugGameState : DebugState<DebugGameStateLayout>
    {
        public JObject FullState => _fullState;

        private DebugStateViewPool<DebugGameStateLayout> _viewPool;
        private DebugStateViewPool<DebugViewAttrLayout> _attrDebugViewPool;
        private DebugStateViewPool<DebugViewObjectLayout> _objectDebugViewPool;
        private DebugStateViewPool<DebugViewAttrLayout> _objectAttrDebugViewPool;
        private RectTransform _content;
        
        private readonly JObject _fullState;

        private readonly IAttrsValue _attrsValue;
        private readonly IObjectsValue _objectsValue;

        public static DebugGameState Create(
            int stateIndex, 
            JObject previousFullState, 
            JObject currentFullState, 
            RectTransform content, 
            DebugStateViewPool<DebugGameStateLayout> viewPool,
            DebugStateViewPool<DebugViewAttrLayout> attrDebugViewPool,
            DebugStateViewPool<DebugViewObjectLayout> objectDebugViewPool,
            DebugStateViewPool<DebugViewAttrLayout> objectAttrDebugViewPool)
        {
            return new DebugGameState(stateIndex, previousFullState, currentFullState, content, viewPool, attrDebugViewPool, objectDebugViewPool, objectAttrDebugViewPool);
        }
        
        private DebugGameState(
            int stateIndex, 
            JObject previousFullState, 
            JObject currentFullState, 
            RectTransform content, 
            DebugStateViewPool<DebugGameStateLayout> viewPool,
            DebugStateViewPool<DebugViewAttrLayout> attrDebugViewPool,
            DebugStateViewPool<DebugViewObjectLayout> objectDebugViewPool,
            DebugStateViewPool<DebugViewAttrLayout> objectAttrDebugViewPool) : base(stateIndex)
        {
            _fullState = currentFullState;
            _content = content;
            _viewPool = viewPool;
            _attrDebugViewPool = attrDebugViewPool;
            _objectDebugViewPool = objectDebugViewPool;
            _objectAttrDebugViewPool = objectAttrDebugViewPool;

            _attrsValue = AttrsValue.Create(currentFullState?.GetValue<JArray>("attrs"),
                previousFullState?.GetValue<JArray>("attrs"));
            _objectsValue = ObjectsValue.Create(currentFullState?.GetValue<JArray>("objects"),
                previousFullState?.GetValue<JArray>("objects"));
        }
        
        public override void Draw(RectTransform content, JObject parameters)
        {
            Layout = _viewPool.Pop();
            Layout.transform.SetParent(_content);
            Layout.UpdatePosition(Vector3.zero);

            switch (parameters.GetEnum<DebugStateViewTypes>("state_view_type"))
            {
                case DebugStateViewTypes.full:
                    DrawFull();
                    break;
                
                case DebugStateViewTypes.delta:
                    DrawDelta();
                    break;
            }

            _content.sizeDelta = Layout.Size;
        }

        private void DrawFull()
        {
            foreach (var attrValue in _attrsValue.Attrs)
            {
                var attrDebugView = _attrDebugViewPool.Pop();
                var currentValue = attrValue.CurrentValue == int.MinValue ? "-" : attrValue.CurrentValue.ToString();
                var oldValue = attrValue.OldValue == int.MinValue ? "-" : attrValue.OldValue.ToString();
                attrDebugView.Apply(attrValue.Key, currentValue, oldValue);
                Layout.PushAttr(attrDebugView);
            }

            foreach (var objectValue in _objectsValue.Objects)
            {
                var objectDebugView = _objectDebugViewPool.Pop();
                objectDebugView.Apply(objectValue.IsCreated, 
                    objectValue.IsDestroyed, 
                    objectValue.Id.ToString(),
                    objectValue.TplId.ToString());

                foreach (var attrValue in objectValue.Attrs.Attrs)
                {
                    var objectAttrDebugView = _objectAttrDebugViewPool.Pop();
                    var currentValue = attrValue.CurrentValue == int.MinValue ? "-" : attrValue.CurrentValue.ToString();
                    var oldValue = attrValue.OldValue == int.MinValue ? "-" : attrValue.OldValue.ToString();
                    objectAttrDebugView.Apply(attrValue.Key, currentValue, oldValue);
                    objectDebugView.PushAttr(objectAttrDebugView);
                }
                
                Layout.PushObject(objectDebugView);
            }
        }

        private void DrawDelta()
        {
            foreach (var attrValue in _attrsValue.Attrs)
            {
                if (!attrValue.IsChange)
                {
                    continue;
                }
                
                var attrDebugView = _attrDebugViewPool.Pop();
                var currentValue = attrValue.CurrentValue == int.MinValue ? "-" : attrValue.CurrentValue.ToString();
                var oldValue = attrValue.OldValue == int.MinValue ? "-" : attrValue.OldValue.ToString();
                attrDebugView.Apply(attrValue.Key, currentValue, oldValue);
                Layout.PushAttr(attrDebugView);
            }

            foreach (var objectValue in _objectsValue.Objects)
            {
                var objectDebugView = _objectDebugViewPool.Pop();
                objectDebugView.Apply(objectValue.IsCreated, 
                    objectValue.IsDestroyed, 
                    objectValue.Id.ToString(),
                    objectValue.TplId.ToString());

                foreach (var attrValue in objectValue.Attrs.Attrs)
                {
                    if (!attrValue.IsChange)
                    {
                        continue;
                    }
                    
                    var objectAttrDebugView = _objectAttrDebugViewPool.Pop();
                    var currentValue = attrValue.CurrentValue == int.MinValue ? "-" : attrValue.CurrentValue.ToString();
                    var oldValue = attrValue.OldValue == int.MinValue ? "-" : attrValue.OldValue.ToString();
                    objectAttrDebugView.Apply(attrValue.Key, currentValue, oldValue);
                    objectDebugView.PushAttr(objectAttrDebugView);
                }
                
                Layout.PushObject(objectDebugView);
            }
        }

        public override void Cleanup()
        {

            while (Layout.TryPopAttr(out var attr))
            {
                attr.Cleanup();
                _attrDebugViewPool.Push(attr);
            }

            while (Layout.TryPopObject(out var obj))
            {
                while (obj.TryPopAttr(out var viewAttrLayout))
                {
                    viewAttrLayout.Cleanup();
                    _objectAttrDebugViewPool.Push(viewAttrLayout);
                }
                
                obj.Cleanup();
                _objectDebugViewPool.Push(obj);
            }
            
            Layout.Cleanup();
            _viewPool.Push(Layout);
        }
    }
}