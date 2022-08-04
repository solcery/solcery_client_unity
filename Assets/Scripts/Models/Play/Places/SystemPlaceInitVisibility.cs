using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Places;
using Solcery.Utils;

namespace Solcery.Models.Play.Places
{
    public interface ISystemPlaceInitVisibility : IEcsInitSystem
    {
    }

    public sealed class SystemPlaceInitVisibility : ISystemPlaceInitVisibility
    {
        public static ISystemPlaceInitVisibility Create()
        {
            return new SystemPlaceInitVisibility();
        }
        
        private SystemPlaceInitVisibility() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var serviceGameContent = systems.GetShared<IGame>().ServiceGameContent;
            var placeHash = new Dictionary<int, JObject>();
            foreach (var placeToken in serviceGameContent.Places)
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
        }
    }
}