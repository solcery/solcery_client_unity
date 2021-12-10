using UnityEngine;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public interface ICardInContainerWidget
    {
        void UpdateParent(Transform parent);
        void UpdatePosition(Vector3 position);
        void Cleanup();
        void Destroy();
    }
}