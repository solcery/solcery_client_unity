using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Tooltips;
using Solcery.Utils;

namespace Solcery.Models.Shared.Initial.Game.Content
{
    public interface ISystemInitialGameContentTooltips : IEcsInitSystem { }
    
    public class SystemInitialGameContentTooltips : ISystemInitialGameContentTooltips
    {
        public static ISystemInitialGameContentTooltips Create()
        {
            return new SystemInitialGameContentTooltips();
        }
        
        private SystemInitialGameContentTooltips() { }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var serviceGameContent = systems.GetShared<IGame>().ServiceGameContent;
            
            var filter = world.Filter<ComponentTooltips>().End();
            var pool = world.GetPool<ComponentTooltips>();

            foreach (var entityId in filter)
            {
                pool.Del(entityId);
            }

            var entityTypeMap = new Dictionary<int, JObject>(serviceGameContent.Tooltips.Count);
            foreach (var tooltipToken in serviceGameContent.Tooltips)
            {
                if (tooltipToken is JObject tooltipObject)
                {
                    entityTypeMap.Add(tooltipObject.GetValue<int>("id"), tooltipObject);
                }
            }
            var entityIndex = world.NewEntity();
            world.GetPool<ComponentTooltips>().Add(entityIndex).Tooltips = entityTypeMap;
        }
    }
}