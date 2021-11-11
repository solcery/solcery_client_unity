using Leopotam.EcsLite;
using UnityEngine;

namespace Solcery.Widgets.Pool
{
    public class PoolObject : MonoBehaviour
    {
        protected EcsWorld EcsWorld;
        
        // call every time when object was got from pool
        public virtual void Init(EcsWorld world)
        {
            EcsWorld = world;
        }

        // call every time when object was returned to pool
        public virtual void Clear()
        {
            EcsWorld = null;
        }
    }
}