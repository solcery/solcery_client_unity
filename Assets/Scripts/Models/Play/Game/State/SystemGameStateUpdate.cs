using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Attributes.Interactable;
using Solcery.Models.Play.Entities;
using Solcery.Models.Play.Game.Attributes;
using Solcery.Models.Play.Game.State.StaticAttributes.Highlighted;
using Solcery.Models.Play.Game.State.StaticAttributes.Interactable;
using Solcery.Models.Play.Game.State.StaticAttributes.Place;
using Solcery.Models.Play.GameState.StaticAttributes;
using Solcery.Models.Play.Triggers;
using Solcery.Utils;

namespace Solcery.Models.Play.Game.State
{
    public interface ISystemGameStateUpdate : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem{ }

    public sealed class SystemGameStateUpdate : ISystemGameStateUpdate
    {
        private readonly IGame _game;
        
        private EcsFilter _filterGameAttributes;
        private EcsFilter _filterEntities;
        private EcsFilter _filterEntityTypes;
        private IStaticAttributes _staticAttributes;
        
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
            _filterEntityTypes = world.Filter<ComponentEntityTypes>().End();
            _staticAttributes = Play.GameState.StaticAttributes.StaticAttributes.Create();
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeHighlighted.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeInteractable.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributePlace.Create());
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
                foreach (var entityId in _filterGameAttributes)
                {
                    ref var gameAttributesComponent = ref world.GetPool<ComponentGameAttributes>().Get(entityId);
                    UpdateAttributes(gameAttributeArray, gameAttributesComponent.Attributes);
                    break;
                }
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
        
        private void UpdateTriggers(int typeId, EcsWorld world, int entityIndex)
        {
            foreach (var uniqEntityTypes in _filterEntityTypes)
            {
                ref var types = ref world.GetPool<ComponentEntityTypes>().Get(uniqEntityTypes);
                if (types.Types.TryGetValue(typeId, out var data))
                {
                    world.GetPool<ComponentTriggers>().Add(entityIndex).Triggers = TriggersData.Parse(data);
                    world.GetPool<ComponentAttributeInteractable>().Add(entityIndex).Value = true;
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
            var typeId = entityData.GetValue<int>("tplId");
            world.GetPool<ComponentEntityType>().Get(entityIndex).Type = typeId;

            // triggers
            UpdateTriggers(typeId, world, entityIndex);
            
            // attributes
            ref var attributesComponent = ref world.GetPool<ComponentEntityAttributes>().Get(entityIndex);
            var attributeArray = entityData.GetValue<JArray>("attrs");
            UpdateAttributes(attributeArray, attributesComponent.Attributes);
            _staticAttributes.ApplyAndUpdateAttributes(world, entityIndex, attributeArray);
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _staticAttributes.Cleanup();
        }
    }
}