using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Places;
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
                    }
                }
            }

            _gameContent = null;
        }
    }
}