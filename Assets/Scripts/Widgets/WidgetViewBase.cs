using Leopotam.EcsLite;
using Solcery.Widgets.Pool;
using UnityEngine;

namespace Solcery.Widgets
{
    public class WidgetViewBase : PoolObject
    {
        public RectTransform RectTransform;
        
        public virtual void Init()
        {
        }

        public override void Clear()
        {
        }
        
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent, false);
        }
        
        public void ApplyAnchor(Vector2 min, Vector2 max)
        {
            RectTransform.anchorMin = min;
            RectTransform.anchorMax = max;
        }        
    }
}
