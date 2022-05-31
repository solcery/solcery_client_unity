using Leopotam.EcsLite;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse
{
    public interface IPlaceWidgetTokenCollection
    {
        bool TryGetTokenPosition(EcsWorld world, int cardId, int slotId, out Vector3 position);
    }
}