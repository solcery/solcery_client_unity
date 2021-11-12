using Leopotam.EcsLite;
using Solcery.Widgets.Pool;
using UnityEngine;

namespace Solcery
{
    public class WidgetBehaviourBase : PoolObject
    {
        protected EcsWorld EcsWorld { get; private set; }
        protected int EntityId { get; private set; }

        public virtual void Init(EcsWorld ecsWorld, int entityId)
        {
            EcsWorld = ecsWorld;
            EntityId = entityId;
        }

        public override void Clear()
        {
            EcsWorld = null;
        }

        public virtual void ApplyAnchor(Vector2 min, Vector2 max)
        {
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
    }
}
