using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Attributes.Interactable;
using Solcery.Models.Shared.Entities;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Game.StaticAttributes;
using Solcery.Models.Shared.Game.StaticAttributes.Highlighted;
using Solcery.Models.Shared.Game.StaticAttributes.Interactable;
using Solcery.Models.Shared.Game.StaticAttributes.Place;
using Solcery.Utils;

namespace Solcery.Models.Simulation.Game.State
{
    public interface ISystemGameStateInitial : IEcsInitSystem { }

    public sealed class SystemGameStateInitial : ISystemGameStateInitial
    {
        private JObject _initialGameState;
        private EcsFilter _filterEntityTypes;
        private IStaticAttributes _staticAttributes;
        
        public static ISystemGameStateInitial Create(JObject initialGameState)
        {
            return new SystemGameStateInitial(initialGameState);
        }
        
        private SystemGameStateInitial(JObject initialGameState)
        {
            _initialGameState = initialGameState;
        }
        
        public void Init(EcsSystems systems)
        {
            if (_initialGameState == null)
            {
                return;
            }
            
            _staticAttributes = StaticAttributes.Create();
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeHighlighted.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeInteractable.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributePlace.Create());
            
            var world = systems.GetWorld();
            _filterEntityTypes = world.Filter<ComponentEntityTypes>().End();
            
            // Update game attributes
            var gameAttributeArray = _initialGameState.TryGetValue("attrs", out JArray attrs) ? attrs : null;
            var entityIndex = world.NewEntity();
            ref var gameAttributesComponent =
                ref world.GetPool<ComponentGameAttributes>().Add(entityIndex);
            gameAttributesComponent.Attributes = new Dictionary<string, int>(gameAttributeArray?.Count ?? 0);
            UpdateAttributes(gameAttributeArray, gameAttributesComponent.Attributes);
            
            // Update game entity
            var entityArray = _initialGameState.GetValue<JArray>("objects");
            var entityHashMap = new Dictionary<int, JObject>(entityArray.Count);
            foreach (var entityToken in entityArray)
            {
                if (entityToken is JObject entityObject)
                {
                    entityHashMap.Add(entityObject.GetValue<int>("id"), entityObject);
                }
            }
            
            foreach (var entityObject in entityHashMap)
            {
                CreateEntity(world, entityObject.Value);
            }
                
            entityHashMap.Clear();
            _staticAttributes.Cleanup();
            _staticAttributes = null;
            _initialGameState = null;
            _filterEntityTypes = null;
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
            
            foreach (var entityTypesId in _filterEntityTypes)
            {
                var entityTypesPool = world.GetPool<ComponentEntityTypes>();
                ref var entityTypes = ref entityTypesPool.Get(entityTypesId);
                if (entityData.TryGetValue("tplId", out int entityTypeId)
                    && entityTypes.Types.TryGetValue(entityTypeId, out var entityTypeData)
                    && entityTypeData.HasKey("picture")
                    && entityTypeData.HasKey("action")
                    && entityTypeData.HasKey("name")
                    && entityTypeData.HasKey("description"))
                {
                    world.GetPool<ComponentEntityCardTag>().Add(entityIndex);
                }
                break;
            }
            
            UpdateEntity(world, entityIndex, entityData);
        }
        
        private void UpdateEntity(EcsWorld world, int entityIndex, JObject entityData)
        {
            world.GetPool<ComponentEntityId>().Get(entityIndex).Id = entityData.GetValue<int>("id");
            var typeId = entityData.GetValue<int>("tplId");
            world.GetPool<ComponentEntityType>().Get(entityIndex).Type = typeId;

            // interactable
            UpdateInteractable(typeId, world, entityIndex);
            
            // attributes
            ref var attributesComponent = ref world.GetPool<ComponentEntityAttributes>().Get(entityIndex);
            var attributeArray = entityData.GetValue<JArray>("attrs");
            UpdateAttributes(attributeArray, attributesComponent.Attributes);
            _staticAttributes.ApplyAndUpdateAttributes(world, entityIndex, attributeArray);
        }
        
        private void UpdateInteractable(int typeId, EcsWorld world, int entityIndex)
        {
            foreach (var uniqEntityTypes in _filterEntityTypes)
            {
                ref var types = ref world.GetPool<ComponentEntityTypes>().Get(uniqEntityTypes);
                if (types.Types.ContainsKey(typeId))
                {
                    world.GetPool<ComponentAttributeInteractable>().Add(entityIndex).Value = true;
                }
            }
        }
    }
}