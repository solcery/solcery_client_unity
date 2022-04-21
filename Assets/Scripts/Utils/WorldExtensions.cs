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
            var objectTypePool = world.GetPool<ComponentObjectId>();
            var objectIdFilter = world.Filter<ComponentObjectId>().Inc<ComponentObjectType>().End();
            foreach (var entityId in objectIdFilter)
            {
                if (objectIdPool.Get(entityId).Id == cardId)
                {
                    type = objectIdPool.Get(entityId).Id;
                    return true;
                }
            }

            type = 0;
            return false;
        }
        
        public static Dictionary<int, JObject> GetCardTypes(this EcsWorld world)
        {
            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            foreach (var objectTypesEntityId in objectTypesFilter)
            {
                return world.GetPool<ComponentObjectTypes>().Get(objectTypesEntityId).Types;
            }

            return null;
        }
    }
}