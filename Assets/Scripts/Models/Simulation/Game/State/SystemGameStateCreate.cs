using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Entities;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Services.LocalSimulation;

namespace Solcery.Models.Simulation.Game.State
{
    public interface ISystemGameStateCreate : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemGameStateCreate : ISystemGameStateCreate
    {
        private IServiceLocalSimulationApplyGameState _applyGameState;
        private EcsFilter _filterGameAttributes;
        private EcsFilter _filterEntities;
        
        public static ISystemGameStateCreate Create(IServiceLocalSimulationApplyGameState applyGameState)
        {
            return new SystemGameStateCreate(applyGameState);
        }
        
        private SystemGameStateCreate(IServiceLocalSimulationApplyGameState applyGameState)
        {
            _applyGameState = applyGameState;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterGameAttributes = world.Filter<ComponentGameAttributes>().End();
            _filterEntities = world.Filter<ComponentEntityTag>().Inc<ComponentEntityId>().Inc<ComponentEntityType>()
                .Inc<ComponentEntityAttributes>().End();
        }
        
        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var gameState = new JObject();
            
            // Game attributes
            foreach (var gameAttributesEntityId in _filterGameAttributes)
            {
                if (!TryCreateAttributeArray(world.GetPool<ComponentGameAttributes>().Get(gameAttributesEntityId).Attributes, out var gameAttributesArray))
                {
                    gameAttributesArray = new JArray();
                }
                
                gameState.Add("attrs", gameAttributesArray);
                
                break;
            }
            
            // Entities
            var entityArray = new JArray();
            var entityIdPool = world.GetPool<ComponentEntityId>();
            var entityTypePool = world.GetPool<ComponentEntityType>();
            var entityAttributesPool = world.GetPool<ComponentEntityAttributes>();
            foreach (var entityId in _filterEntities)
            {
                if (!TryCreateAttributeArray(entityAttributesPool.Get(entityId).Attributes, out var attributesArray))
                {
                    continue;
                }
                
                entityArray.Add(new JObject
                {
                    ["id"] = new JValue(entityIdPool.Get(entityId).Id),
                    ["tplId"] = new JValue(entityTypePool.Get(entityId).Type),
                    ["attrs"] = attributesArray
                });
            }
            gameState.Add("objects", entityArray);
            
            _applyGameState.ApplySimulatedGameState(gameState);
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _applyGameState = null;
        }

        private bool TryCreateAttributeArray(Dictionary<string, int> attributesHashMap, out JArray attributeArray)
        {
            if (attributesHashMap == null || attributesHashMap.Count <= 0)
            {
                attributeArray = null;
                return false;
            }

            attributeArray = new JArray();
            foreach (var attribute in attributesHashMap)
            {
                attributeArray.Add(new JObject
                {
                    ["key"] = new JValue(attribute.Key),
                    ["value"] = new JValue(attribute.Value)
                });
            }
            
            return true;
        }
    }
}