using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Places;
using Solcery.Widgets_new;

namespace Solcery.Utils
{
    public static class WorldExtensions
    {
        public static PlaceWidget GetPlaceWidget(this EcsWorld world, int placeId)
        {
            var placeWidgetNewPool = world.GetPool<ComponentPlaceWidgetNew>();
            var placeIdPool = world.GetPool<ComponentPlaceId>();
            var widgetFilter = placeIdPool.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().Inc<ComponentPlaceWidgetNew>().End();
            foreach (var entityId in widgetFilter)
            {
                if (placeIdPool.Get(entityId).Id == placeId)
                {
                    return placeWidgetNewPool.Get(entityId).Widget;
                }
            }

            return null;
        }

        public static PlaceWidget GetPlaceWidgetForObjectId(this EcsWorld world, int objectId)
        {
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectIdFilter = world.Filter<ComponentObjectId>().End();
            var attributesPool = world.GetPool<ComponentObjectAttributes>();
            foreach (var entityId in objectIdFilter)
            {
                if (objectIdPool.Get(entityId).Id == objectId)
                {
                    var attributes = attributesPool.Get(entityId).Attributes;
                    if (attributes.TryGetValue(GameJsonKeys.Place, out var placeValue))
                    {
                        return world.GetPlaceWidget(placeValue.Current);
                    }
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
                    type = objectTypePool.Get(entityId).TplId;
                    return true;
                }
            }

            type = 0;
            return false;
        }
    }
}