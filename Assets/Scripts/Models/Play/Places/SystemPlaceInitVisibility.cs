using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Places;
using Solcery.Utils;

namespace Solcery.Models.Play.Places
{
    public interface ISystemPlaceInitVisibility : IEcsInitSystem
    {
    }

    public sealed class SystemPlaceInitVisibility : ISystemPlaceInitVisibility
    {
        private JObject _gameContent;
        
        public static ISystemPlaceInitVisibility Create(JObject gameContent)
        {
            return new SystemPlaceInitVisibility(gameContent);
        }
        
        private SystemPlaceInitVisibility(JObject gameContent)
        {
            _gameContent = gameContent;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var placeHash = new Dictionary<int, JObject>();
            if (_gameContent.TryGetValue("places", out JObject placesObject)
                && placesObject.TryGetValue("objects", out JArray placeArray))
            {
                foreach (var placeToken in placeArray)
                {
                    if (placeToken is JObject placeObject 
                        && placeObject.TryGetValue("place_id", out int placeId))
                    {
                        if (placeObject.TryGetValue("visibility_condition", out JObject visibilityBrick))
                        {
                            placeHash.Add(placeId, visibilityBrick);
                            continue;
                        }
                        
                        placeHash.Add(placeId, null);
                    }
                }
            }
            
            var filterPlace = world.Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().End();
            var poolPlaceId = world.GetPool<ComponentPlaceId>();
            var poolPlaceVisible = world.GetPool<ComponentPlaceIsVisible>();
            var poolPlaceVisibilityBrick = world.GetPool<ComponentPlaceVisibilityBrick>();
            foreach (var placeEntityId in filterPlace)
            {
                var placeId = poolPlaceId.Get(placeEntityId).Id;
                poolPlaceVisible.Add(placeEntityId);
                poolPlaceVisibilityBrick.Add(placeEntityId).VisibilityBrick =
                    placeHash.ContainsKey(placeId) ? placeHash[placeId] : null;
            }

            _gameContent = null;
        }
    }
}