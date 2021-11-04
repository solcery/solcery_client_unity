using Leopotam.EcsLite;
using Solcery.Services.Widget;
using UnityEngine;

namespace Solcery
{
    public class UiBaseWidget : PoolObject, IVisible
    {
        public RectTransform RectTransform;
        
        public override void Init(EcsWorld world)
        {
            base.Init(world);
            transform.position = Vector3.zero;
        }
        
        public virtual void SetVisible(bool value)
        {
            gameObject.gameObject.SetActive(value);
        }
    }
}
