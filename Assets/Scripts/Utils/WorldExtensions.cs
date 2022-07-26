using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Places;
using Solcery.Widgets_new;

namespace Solcery.Utils
{
    public static class WorldExtensions
    {
        public static PlaceWidget GetPlaceWidget(this EcsWorld world, int fromPlaceId)
        {
            var placeWidgetNewPool = world.GetPool<ComponentPlaceWidgetNew>();
            var placeIdPool = world.GetPool<ComponentPlaceId>();
            var widgetFilter = placeIdPool.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().Inc<ComponentPlaceWidgetNew>().End();
            foreach (var entityId in widgetFilter)
            {
                if (placeIdPool.Get(entityId).Id == fromPlaceId)
                {
                    return placeWidgetNewPool.Get(entityId).Widget;
                }
            }

            return null;
        }

        public static bool TryGetCardTypeByCardId(this EcsWorld world, int cardId, out int type)
        {
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var objectIdFilter = world.Filter<ComponentObjectId>().Inc<ComponentObjectType>().End();
            foreach (var entityId in objectIdFilter)
            {
                if (objectIdPool.Get(entityId).Id == cardId)
                {
                    type = objectTypePool.Get(entityId).Type;
                    return true;
                }
            }

            type = 0;
            return false;
        }
    }
}