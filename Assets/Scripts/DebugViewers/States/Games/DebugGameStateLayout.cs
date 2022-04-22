using System;
using System.Collections.Generic;
using Solcery.DebugViewers.Views.Attrs;
using Solcery.DebugViewers.Views.Objects;
using UnityEngine;

namespace Solcery.DebugViewers.States.Games
{
    public sealed class DebugGameStateLayout : MonoBehaviour
    {
        public Vector2 Size => _content.sizeDelta;
        
        [SerializeField]
        private RectTransform attrs;
        [SerializeField]
        private RectTransform objects;

        private RectTransform _content;
        private Stack<DebugViewAttrLayout> _attrs;
        private Stack<DebugViewObjectLayout> _objects;

        private void Awake()
        {
            _content = GetComponent<RectTransform>();
        }

        public void Cleanup()
        {
            if (_attrs.Count > 0)
            {
                throw new Exception("_attrs.Count greater 0, call TryPopAttr and Cleanup all attrs!");
            }
            
            if (_objects.Count > 0)
            {
                throw new Exception("_objects.Count greater 0, call TryPopObject and Cleanup all objects!");
            }
        }

        public void PushAttr(DebugViewAttrLayout viewAttrLayout)
        {
            _attrs ??= new Stack<DebugViewAttrLayout>();
            _attrs.Push(viewAttrLayout);
            viewAttrLayout.transform.SetParent(attrs);
            
            viewAttrLayout.UpdatePosition(new Vector3(0, -attrs.sizeDelta.y));

            var oldSize = attrs.sizeDelta;
            var newSize = new Vector2(oldSize.x, oldSize.y + viewAttrLayout.Size.y);
            attrs.sizeDelta = newSize;

            UpdateObjectsFramePosition(Mathf.Abs(attrs.localPosition.y) + newSize.y);
            UpdateContentSize();
        }
        
        public bool TryPopAttr(out DebugViewAttrLayout viewAttrLayout)
        {
            _attrs ??= new Stack<DebugViewAttrLayout>();
            var result = _attrs.TryPop(out viewAttrLayout);
            
            if (result)
            {
                var oldSize = attrs.sizeDelta;
                var newSize = new Vector2(oldSize.x, oldSize.y - viewAttrLayout.Size.y);
                attrs.sizeDelta = newSize;
                
                UpdateObjectsFramePosition(Mathf.Abs(attrs.localPosition.y) + newSize.y);
                UpdateContentSize();
            }
            
            return result;
        }
        
        public void PushObject(DebugViewObjectLayout viewObjectLayout)
        {
            _objects ??= new Stack<DebugViewObjectLayout>();
            _objects.Push(viewObjectLayout);
            viewObjectLayout.transform.SetParent(objects);
            
            viewObjectLayout.UpdatePosition(new Vector3(0, -objects.sizeDelta.y));
            
            var oldSize = objects.sizeDelta;
            var newSize = new Vector2(oldSize.x, oldSize.y + viewObjectLayout.Size.y);
            objects.sizeDelta = newSize;
            
            UpdateContentSize();
        }
        
        public bool TryPopObject(out DebugViewObjectLayout viewObjectLayout)
        {
            _objects ??= new Stack<DebugViewObjectLayout>();
            var result = _objects.TryPop(out viewObjectLayout);

            if (result)
            {
                var oldSize = objects.sizeDelta;
                var newSize = new Vector2(oldSize.x, oldSize.y - viewObjectLayout.Size.y);
                objects.sizeDelta = newSize;
                
                UpdateContentSize();
            }
            
            return result;
        }

        public void UpdatePosition(Vector3 position)
        {
            _content.localPosition = position;
        }

        private void UpdateObjectsFramePosition(float deltaY)
        {
            var oldPosition = objects.localPosition;
            var newPosition = new Vector3(oldPosition.x, -deltaY, oldPosition.z);
            objects.localPosition = newPosition;
        }

        private void UpdateContentSize()
        {
            var oldSize = Size;
            var newSize = new Vector2(oldSize.x, Mathf.Abs(objects.localPosition.y) + objects.sizeDelta.y);
            _content.sizeDelta = newSize;
        }
    }
}