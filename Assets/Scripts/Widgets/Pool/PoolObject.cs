using Leopotam.EcsLite;
using UnityEngine;

namespace Solcery.Widgets.Pool
{
    public class PoolObject : MonoBehaviour
    {
        // call every time when object was returned to pool
        public virtual void Clear()
        {
        }
    }
}