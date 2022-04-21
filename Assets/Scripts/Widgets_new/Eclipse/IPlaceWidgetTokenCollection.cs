using UnityEngine;

namespace Solcery.Widgets_new.Eclipse
{
    public interface IPlaceWidgetTokenCollection
    {
        bool TryGetTokenPosition(int cardId, int slotId, out Vector3 position);
    }
}