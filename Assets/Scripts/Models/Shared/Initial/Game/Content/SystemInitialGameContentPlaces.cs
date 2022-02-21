using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.DragDrop.Parameters;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Places;
using Solcery.Utils;

namespace Solcery.Models.Shared.Initial.Game.Content
{
    public interface ISystemInitialGameContentPlaces : IEcsInitSystem { }

    public sealed class SystemInitialGameContentPlaces : ISystemInitialGameContentPlaces
    {
        private JObject _gameContent;

        public static ISystemInitialGameContentPlaces Create(JObject gameContent)
        {
            return new SystemInitialGameContentPlaces(gameContent);
        }
        
        private SystemInitialGameContentPlaces(JObject gameContent)
        {
            _gameContent = gameContent;
        }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            if (_gameContent == null)
            {
                return;
            }
            
            var world = systems.GetWorld();
            
            // Только тут мы инитим place, так что можно спокойно удалить если что то есть еще
            var filter = world.Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().End();
            var dragDropFilter = world
                .Filter<ComponentDragDropParametersTag>()
                .Inc<ComponentDragDropParametersId>()
                .End();
            var dragDropIdPool = world.GetPool<ComponentDragDropParametersId>();

            foreach (var entityId in filter)
            {
                world.DelEntity(entityId);
            }

            if (_gameContent.TryGetValue("places", out JObject placesObject)
                && placesObject.TryGetValue("objects", out JArray placeArray))
            {
                foreach (var placeToken in placeArray)
                {
                    if (placeToken is JObject placeObject)
                    {
                        var entityIndex = world.NewEntity();
                        world.GetPool<ComponentPlaceTag>().Add(entityIndex);
                        world.GetPool<ComponentPlaceId>().Add(entityIndex).Id =
                            placeObject.GetValue<int>("placeId");
                        if (placeObject.TryGetValue("drag_n_drop", out int dragDropId))
                        {
                            foreach (var dragDropEntityId in dragDropFilter)
                            {
                                if (dragDropIdPool.Get(dragDropEntityId).Id != dragDropId)
                                {
                                    continue;
                                }

                                world.GetPool<ComponentPlaceDragDropEntityId>().Add(entityIndex).DragDropEntityId =
                                    dragDropEntityId;
                                break;
                            }
                            
                        }
                    }
                }
            }

            _gameContent = null;
        }
    }
}