using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Places;
using Solcery.Utils;

namespace Solcery.Models.Play.Places
{
    public interface ISystemPlaceInitAvailability : IEcsInitSystem
    {
    }
    
    public class SystemPlaceInitAvailability : ISystemPlaceInitAvailability
    {
        public static ISystemPlaceInitAvailability Create()
        {
            return new SystemPlaceInitAvailability();
        }
        
        private SystemPlaceInitAvailability() { }

        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var serviceGameContent = systems.GetShared<IGame>().ServiceGameContent;
            var placeHash = new Dictionary<int, JObject>();
            foreach (var placeToken in serviceGameContent.Places)
            {
                if (placeToken is JObject placeObject 
                    && placeObject.TryGetValue(GameJsonKeys.PlaceId, out int placeId))
                {
                    if (placeObject.TryGetValue(GameJsonKeys.AvailabilityConditionBrick, out JObject availableBrick))
                    {
                        placeHash.Add(placeId, availableBrick);
                        continue;
                    }
                        
                    placeHash.Add(placeId, null);
                }
            }
            
            var filterPlace = world.Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().End();
            var poolPlaceId = world.GetPool<ComponentPlaceId>();
            var poolPlaceAvailable = world.GetPool<ComponentPlaceIsAvailable>();
            var poolPlaceAvailableBrick = world.GetPool<ComponentPlaceAvailableBrick>();
            foreach (var placeEntityId in filterPlace)
            {
                var placeId = poolPlaceId.Get(placeEntityId).Id;
                poolPlaceAvailable.Add(placeEntityId);
                poolPlaceAvailableBrick.Add(placeEntityId).AvailableBrick =
                    placeHash.ContainsKey(placeId) ? placeHash[placeId] : null;
            }
        }
    }
}
