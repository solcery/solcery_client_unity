using UnityEngine;

namespace Solcery.Widgets_new.Cards.Pools
{
    public interface IPoolingWidget
    {
        void UpdateParent(Transform parent);
        void Cleanup();
        void Destroy();
        void BackToPool();
    }
}