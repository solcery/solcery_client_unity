using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Places;
using Solcery.Utils;

namespace Solcery.Models.Play.Initial.Game.Content
{
    public interface ISystemInitialGameContentPlaceWidgets : IEcsInitSystem { }

    public sealed class SystemInitialGameContentPlaceWidgets : ISystemInitialGameContentPlaceWidgets
    {
        public static ISystemInitialGameContentPlaceWidgets Create()
        {
            return new SystemInitialGameContentPlaceWidgets();
        }
        
        private SystemInitialGameContentPlaceWidgets() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            
            var placeHashMap = new Dictionary<int, JObject>(game.ServiceGameContent.Places.Count);
                
            foreach (var placeToken in game.ServiceGameContent.Places)
            {
                if (placeToken is JObject placeObject 
                    && placeObject.TryGetValue(GameJsonKeys.PlaceId, out int placeId))
                {
                    placeHashMap.Add(placeId, placeObject);
                }
            }

            if (placeHashMap.Count > 0)
            {
                var world = systems.GetWorld();
                var filter = world.Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().End();
                var poolPlaceId = world.GetPool<ComponentPlaceId>();
                var poolPlaceWidgetNew = world.GetPool<ComponentPlaceWidgetNew>();

                foreach (var entityId in filter)
                {
                    if (placeHashMap.TryGetValue(poolPlaceId.Get(entityId).Id, out var placeObject))
                    {
                        // TODO: New place widgets
                        if (game.PlaceWidgetFactory.TryCreatePlaceWidgetByType(placeObject, out var placeWidget))
                        {
                            poolPlaceWidgetNew.Add(entityId).Widget = placeWidget;
                            placeWidget.UpdatePlaceId(poolPlaceId.Get(entityId).Id);
                            placeWidget.UpdateLinkedEntityId(entityId);
                        }
                    }
                }
            }
        }
    }
}