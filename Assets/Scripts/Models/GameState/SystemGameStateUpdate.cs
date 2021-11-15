using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Entities;
using Solcery.Models.Game;
using Solcery.Utils;


namespace Solcery.Models.GameState
{
    public interface ISystemGameStateUpdate : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemGameStateUpdate : ISystemGameStateUpdate
    {
        private readonly IGame _game;
        
        private EcsFilter _filterGameAttributes;
        private EcsFilter _filterEntities;
        
        public static ISystemGameStateUpdate Create(IGame game)
        {
            return new SystemGameStateUpdate(game);
        }

        private SystemGameStateUpdate(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterGameAttributes = world.Filter<ComponentGameAttributes>().End();
            _filterEntities = world.Filter<ComponentEntityTag>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var gameStateJson = _game.GameStatePopAndClear;

            if (gameStateJson == null)
            {
                return;
            }

            var world = systems.GetWorld();
            
            // Add Component Game State Update Tag
            world.GetPool<ComponentGameStateUpdateTag>().Add(world.NewEntity());
            
            // Update game attributes
            var gameAttributeArray = gameStateJson.TryGetValue("attrs", out JArray attrs) ? attrs : null;
            if (_filterGameAttributes.GetEntitiesCount() <= 0)
            {
                var entityIndex = world.NewEntity();
                ref var gameAttributesComponent =
                    ref world.GetPool<ComponentGameAttributes>().Add(entityIndex);
                gameAttributesComponent.Attributes = new Dictionary<string, int>(gameAttributeArray?.Count ?? 0);
                UpdateAttributes(gameAttributeArray, gameAttributesComponent.Attributes);
            }
            else
            {
                ref var gameAttributesComponent = ref world.GetPool<ComponentGameAttributes>()
                    .Get(_filterGameAttributes.GetRawEntities()[0]);
                UpdateAttributes(gameAttributeArray, gameAttributesComponent.Attributes);
            }
            
            // Update game entity
            var entityArray = gameStateJson.GetValue<JArray>("objects");
            var entityHashMap = new Dictionary<int, JObject>(entityArray.Count);
            foreach (var entityToken in entityArray)
            {
                if (entityToken is JObject entityObject)
                {
                    entityHashMap.Add(entityObject.GetValue<int>("id"), entityObject);
                }
            }

            foreach (var entityIndex in _filterEntities)
            {
                var entityId = world.GetPool<ComponentEntityId>().Get(entityIndex).Id;

                if (!entityHashMap.ContainsKey(entityId))
                {
                    world.DelEntity(entityIndex);
                    continue;
                }

                UpdateEntity(world, entityId, entityHashMap[entityId]);
                entityHashMap.Remove(entityId);
            }

            foreach (var entityObject in entityHashMap)
            {
                CreateEntity(world, entityObject.Value);
            }
                
            entityHashMap.Clear();
        }
        
        private void UpdateAttributes(JArray gameAttributeArray, Dictionary<string, int> attributesHashMap)
        {
            if (gameAttributeArray == null)
            {
                attributesHashMap?.Clear();
                return;
            }

            foreach (var gameAttributeToken in gameAttributeArray)
            {
                if (gameAttributeToken is JObject gameAttributeObject)
                {
                    var key = gameAttributeObject.GetValue<string>("key");
                    var value = gameAttributeObject.GetValue<int>("value");
                    if (!attributesHashMap.ContainsKey(key))
                    {
                        attributesHashMap.Add(key, value);
                        continue;
                    }

                    attributesHashMap[key] = value;
                }
            }
        }
        
        private void CreateEntity(EcsWorld world, JObject entityData)
        {
            var entityIndex = world.NewEntity();
            world.GetPool<ComponentEntityTag>().Add(entityIndex);
            world.GetPool<ComponentEntityId>().Add(entityIndex);
            world.GetPool<ComponentEntityType>().Add(entityIndex);
            world.GetPool<ComponentEntityAttributes>().Add(entityIndex).Attributes =
                new Dictionary<string, int>();
            
            UpdateEntity(world, entityIndex, entityData);
        }

        private void UpdateEntity(EcsWorld world, int entityIndex, JObject entityData)
        {
            world.GetPool<ComponentEntityId>().Get(entityIndex).Id = entityData.GetValue<int>("id");
            world.GetPool<ComponentEntityType>().Get(entityIndex).Type = entityData.GetValue<int>("tplId");
            
            ref var attributesComponent = ref world.GetPool<ComponentEntityAttributes>().Get(entityIndex);
            UpdateAttributes(entityData.GetValue<JArray>("attrs"), attributesComponent.Attributes);
        }
    }
}