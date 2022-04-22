using Leopotam.EcsLite;
using Solcery.Widgets_new.Eclipse;
using UnityEngine;

namespace Solcery.Utils
{
    public static class WidgetExtensions
    {
        public static bool TryGetTokenFromPosition(EcsWorld world, int fromPlaceId, int fromCardId, int slotId, out Vector3 position)
        {
            var placeWidget = world.GetPlaceWidget(fromPlaceId);
            if (placeWidget != null && placeWidget is IPlaceWidgetTokenCollection widget)
            {
                if (widget.TryGetTokenPosition(world, fromCardId, slotId, out position))
                {
                    return true;
                }
            }
            
            position = Vector3.zero;
            return false;
        }
    }
}