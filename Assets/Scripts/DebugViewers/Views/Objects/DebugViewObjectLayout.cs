using System;
using System.Collections.Generic;
using Solcery.DebugViewers.Views.Attrs;
using TMPro;
using UnityEngine;

namespace Solcery.DebugViewers.Views.Objects
{
    public sealed class DebugViewObjectLayout : MonoBehaviour
    {
        public Vector2 Size => content.sizeDelta;
        
        [SerializeField]
        private GameObject header;
        [SerializeField]
        private GameObject created;
        [SerializeField]
        private GameObject destroyed;
        [SerializeField]
        private TMP_Text id;
        [SerializeField]
        private TMP_Text tplId;
        [SerializeField]
        private RectTransform content;

        private Stack<DebugViewAttrLayout> _attrs;

        public void Apply(bool isCreated, bool isDestroyed, string id, string tplId)
        {
            header.SetActive(isCreated || isDestroyed);
            created.SetActive(isCreated);
            destroyed.SetActive(isDestroyed);
            
            this.id.text = id;
            this.tplId.text = tplId;
        }

        public void Cleanup()
        {
            id.text = "";
            tplId.text = "";

            if (_attrs.Count > 0)
            {
                throw new Exception("_attrs.Count greater 0, call TryPopAttr and Cleanup all attrs!");
            }
        }

        public void PushAttr(DebugViewAttrLayout viewAttrLayout)
        {
            _attrs ??= new Stack<DebugViewAttrLayout>();
            _attrs.Push(viewAttrLayout);
            viewAttrLayout.transform.SetParent(content);
            viewAttrLayout.UpdatePosition(new Vector3(0, -Size.y));

            var oldSize = content.sizeDelta;
            var newSize = new Vector2(oldSize.x, oldSize.y + viewAttrLayout.Size.y);
            content.sizeDelta = newSize;
        }

        public bool TryPopAttr(out DebugViewAttrLayout viewAttrLayout)
        {
            _attrs ??= new Stack<DebugViewAttrLayout>();
            var result = _attrs.TryPop(out viewAttrLayout);

            if (result)
            {
                var oldSize = content.sizeDelta;
                var newSize = new Vector2(oldSize.x, oldSize.y - viewAttrLayout.Size.y);
                content.sizeDelta = newSize;
            }
            
            return result;
        }
        
        public void UpdatePosition(Vector3 position)
        {
            content.localPosition = position;
        }
    }
}