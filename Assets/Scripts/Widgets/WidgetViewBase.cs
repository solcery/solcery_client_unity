using Solcery.Widgets.Deck;
using Solcery.Widgets.Pool;
using UnityEngine;

namespace Solcery.Widgets
{
    public abstract class WidgetViewBase : PoolObject
    {
        public RectTransform RectTransform;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _pivot;
        private Vector2 _anchoredPosition;
        private Vector2 _offsetMax;
        private Vector2 _offsetMin;
        
        public override void Create()
        {
            _anchoredPosition = RectTransform.anchoredPosition;
            _anchorMin = RectTransform.anchorMin;
            _anchorMax = RectTransform.anchorMax;
            _pivot = RectTransform.pivot;
            _offsetMin = RectTransform.offsetMin;
            _offsetMax = RectTransform.offsetMax;
        }
        
        public virtual void Init()
        {
            RectTransform.offsetMax = Vector2.down;
            RectTransform.anchoredPosition = _anchoredPosition;
            RectTransform.pivot = _pivot;
            RectTransform.anchorMin = _anchorMin;
            RectTransform.anchorMax = _anchorMax;
            RectTransform.offsetMin = _offsetMin;
            RectTransform.offsetMax = _offsetMax;
        }

        public override void Clear()
        {
        }
        
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent, false);
        }

        public abstract void ApplyPlaceViewData(WidgetPlaceViewData viewData);
    }
}
