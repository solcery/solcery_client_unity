using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Tooltips;
using Solcery.Utils;

namespace Solcery.Models.Shared.Initial.Game.Content
{
    public interface ISystemInitialGameContentTooltips : IEcsInitSystem { }
    
    public class SystemInitialGameContentTooltips : ISystemInitialGameContentTooltips
    {
        private JObject _gameContent;

        public static ISystemInitialGameContentTooltips Create(JObject gameContent)
        {
            return new SystemInitialGameContentTooltips(gameContent);
        }
        
        private SystemInitialGameContentTooltips(JObject gameContent)
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
            
            var filter = world.Filter<ComponentTooltips>().End();
            var pool = world.GetPool<ComponentTooltips>();

            foreach (var entityId in filter)
            {
                pool.Del(entityId);
            }

            if (_gameContent.TryGetValue("tooltips", out JObject tooltipsObject)
                && tooltipsObject.TryGetValue("objects", out JArray tooltipArray))
            {
                var entityTypeMap = new Dictionary<int, JObject>(tooltipArray.Count);
                foreach (var tooltipToken in tooltipArray)
                {
                    if (tooltipToken is JObject tooltipObject)
                    {
                        entityTypeMap.Add(tooltipObject.GetValue<int>("tooltip_id"), tooltipObject);
                    }
                }
                var entityIndex = world.NewEntity();
                world.GetPool<ComponentTooltips>().Add(entityIndex).Tooltips = entityTypeMap;
            }

            _gameContent = null;
        }
    }
}