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
        private IGame _game;

        public static ISystemInitialGameContentPlaceWidgets Create(IGame game)
        {
            return new SystemInitialGameContentPlaceWidgets(game);
        }
        
        private SystemInitialGameContentPlaceWidgets(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            if (_game == null)
            {
                return;
            }
            
            if (_game.GameContent.TryGetValue("places", out JObject placesObject) 
                && placesObject.TryGetValue("objects", out JArray placeArray))
            {
                var placeHashMap = new Dictionary<int, JObject>(placeArray.Count);
                
                foreach (var placeToken in placeArray)
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
                            if (_game.PlaceWidgetFactory.TryCreatePlaceWidgetByType(placeObject, out var placeWidget))
                            {
                                poolPlaceWidgetNew.Add(entityId).Widget = placeWidget;
                                placeWidget.UpdatePlaceId(poolPlaceId.Get(entityId).Id);
                                placeWidget.UpdateLinkedEntityId(entityId);
                            }
                        }
                    }
                }
            }

            _game = null;
        }
    }
}