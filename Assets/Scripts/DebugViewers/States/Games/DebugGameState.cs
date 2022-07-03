using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary.Game;
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
        private DebugStateViewPool<DebugGameStateLayout> _viewPool;
        private DebugStateViewPool<DebugViewAttrLayout> _attrDebugViewPool;
        private DebugStateViewPool<DebugViewObjectLayout> _objectDebugViewPool;
        private DebugStateViewPool<DebugViewAttrLayout> _objectAttrDebugViewPool;
        private RectTransform _content;

        private readonly IAttrsValue _attrsValue;
        private readonly IObjectsValue _objectsValue;
        private readonly Dictionary<string, Vector2> _keyToPosition;

        public static DebugGameState Create(
            DebugUpdateGameStateBinary binary, 
            RectTransform content, 
            DebugStateViewPool<DebugGameStateLayout> viewPool,
            DebugStateViewPool<DebugViewAttrLayout> attrDebugViewPool,
            DebugStateViewPool<DebugViewObjectLayout> objectDebugViewPool,
            DebugStateViewPool<DebugViewAttrLayout> objectAttrDebugViewPool)
        {
            return new DebugGameState(binary, content, viewPool, attrDebugViewPool, objectDebugViewPool, objectAttrDebugViewPool);
        }
        
        private DebugGameState(
            DebugUpdateGameStateBinary binary, 
            RectTransform content, 
            DebugStateViewPool<DebugGameStateLayout> viewPool,
            DebugStateViewPool<DebugViewAttrLayout> attrDebugViewPool,
            DebugStateViewPool<DebugViewObjectLayout> objectDebugViewPool,
            DebugStateViewPool<DebugViewAttrLayout> objectAttrDebugViewPool) : base(binary.Index)
        {
            _content = content;
            _viewPool = viewPool;
            _attrDebugViewPool = attrDebugViewPool;
            _objectDebugViewPool = objectDebugViewPool;
            _objectAttrDebugViewPool = objectAttrDebugViewPool;
            _keyToPosition = new Dictionary<string, Vector2>();

            _attrsValue = AttrsValue.Create(binary.Attrs);
            _objectsValue = ObjectsValue.Create(binary.Objects);
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
                
                _keyToPosition.Add(objectValue.Id.ToString(), Layout.PushObject(objectDebugView));
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
                if (objectValue.Attrs.Attrs.Count(o => o.IsChange) <= 0)
                {
                    continue;
                }
                
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
                
                _keyToPosition.Add(objectValue.Id.ToString(), Layout.PushObject(objectDebugView));
            }
        }

        public override void Cleanup()
        {
            _keyToPosition.Clear();

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

        public override IReadOnlyList<string> AllMoveToKeys()
        {
            return _keyToPosition.Keys.ToList();
        }

        public override Vector2 GetPositionToKeys(string key)
        {
            if (_keyToPosition.TryGetValue(key, out var position))
            {
                return new Vector2(0f, Mathf.Abs(position.y));
            }

            return new Vector2(0f, 0f);
        }
    }
}