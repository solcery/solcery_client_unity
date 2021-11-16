using Solcery.Widgets.Deck;
using Solcery.Widgets.Pool;
using UnityEngine;

namespace Solcery.Widgets
{
    public abstract class WidgetViewBase : PoolObject
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

        public abstract void ApplyPlaceViewData(WidgetPlaceViewData viewData);
    }
}
