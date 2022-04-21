using Leopotam.EcsLite;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Places;
using Solcery.Widgets_new;

namespace Solcery.Utils
{
    public static class WorldExtensions
    {
        public static PlaceWidget GetPlaceWidget(this EcsWorld world, int fromPlaceId)
        {
            var poolPlaceWidgetNew = world.GetPool<ComponentPlaceWidgetNew>();
            var poolPlaceId = world.GetPool<ComponentPlaceId>();
            var widgetFilter = poolPlaceId.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().Inc<ComponentPlaceWidgetNew>().End();
            foreach (var entityId in widgetFilter)
            {
                if (poolPlaceId.Get(entityId).Id == fromPlaceId)
                {
                    return poolPlaceWidgetNew.Get(entityId).Widget;
                }
            }

            return null;
        }
        
    }
}