#if UNITY_EDITOR || LOCAL_SIMULATION
using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Attributes.Interactable;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Game.StaticAttributes;
using Solcery.Models.Shared.Game.StaticAttributes.Highlighted;
using Solcery.Models.Shared.Game.StaticAttributes.Interactable;
using Solcery.Models.Shared.Game.StaticAttributes.Place;
using Solcery.Models.Shared.Objects;
using Solcery.Services.LocalSimulation.GameStates;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Simulation.Game.State
{
    public interface ISystemGameStateRollback : IEcsRunSystem
    { }

    public sealed class SystemGameStateRollback : ISystemGameStateRollback
    {
        private readonly IServiceGameState _serviceGameState;
        private EcsFilter _filterObjectIdHash;
        private IStaticAttributes _staticAttributes;

        public static ISystemGameStateRollback Create(IServiceGameState serviceGameState)
        {
            return new SystemGameStateRollback(serviceGameState);
        }

        private SystemGameStateRollback(IServiceGameState serviceGameState)
        {
            _serviceGameState = serviceGameState;
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (!_serviceGameState.TryPopGameState(out var gameState))
            {
                return;
            }
            
            var world = systems.GetWorld();
            var game = systems.GetShared<IGame>();

            // Delete all entities
            {
                var entities = new int[world.GetEntitiesCount()];
                var entityCount = world.GetAllEntities(ref entities);
                for (var entityIndex = 0; entityIndex < entityCount; entityIndex++)
                {
                    var entity = entities[entityIndex];
                    world.DelEntity(entity);
                }
            }

            {
                _staticAttributes = StaticAttributes.Create();
                _staticAttributes.RegistrationStaticAttribute(StaticAttributeHighlighted.Create());
                _staticAttributes.RegistrationStaticAttribute(StaticAttributeInteractable.Create());
                _staticAttributes.RegistrationStaticAttribute(StaticAttributePlace.Create());
                _filterObjectIdHash = world.Filter<ComponentObjectIdHash>().End();
                var objectIdHashPool = world.GetPool<ComponentObjectIdHash>();

                foreach (var oih in _filterObjectIdHash)
                {
                    world.DelEntity(oih);
                }

                var objectIdHashPoolEntityId = world.NewEntity();
                var objectIdHashes = objectIdHashPool.Add(objectIdHashPoolEntityId).ObjectIdHashes;
                
                // Update game attributes
                var gameAttributeArray = gameState.TryGetValue("attrs", out JArray attrs) ? attrs : null;
                var entityIndex = world.NewEntity();
                ref var gameAttributesComponent =
                    ref world.GetPool<ComponentGameAttributes>().Add(entityIndex);
                UpdateAttributes(gameAttributeArray, gameAttributesComponent.Attributes);
                
                // Update game entity
                var entityArray = gameState.GetValue<JArray>("objects");
                var entityHashMap = new Dictionary<int, JObject>(entityArray.Count);
                foreach (var entityToken in entityArray)
                {
                    if (entityToken is JObject entityObject)
                    {
                        entityHashMap.Add(entityObject.GetValue<int>("id"), entityObject);
                    }
                }

                var maxObjectId = 0;
                
                foreach (var entityObject in entityHashMap)
                {
                    maxObjectId = Mathf.Max(maxObjectId, entityObject.Key);
                    CreateEntity(world, game, entityObject.Value);
                }
                
                objectIdHashes.UpdateHeadId(maxObjectId);
                    
                entityHashMap.Clear();
                _staticAttributes.Cleanup();
                _staticAttributes = null;
            }
        }
        
        private void UpdateAttributes(JArray gameAttributeArray, Dictionary<string, IAttributeValue> attributesHashMap)
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
                        attributesHashMap.Add(key, AttributeValue.Create(value));
                        continue;
                    }

                    attributesHashMap[key].UpdateValue(value);
                }
            }
        }
        
        private void CreateEntity(EcsWorld world, IGame game, JObject entityData)
        {
            var entityIndex = world.NewEntity();
            world.GetPool<ComponentObjectTag>().Add(entityIndex);
            world.GetPool<ComponentObjectId>().Add(entityIndex);
            world.GetPool<ComponentObjectType>().Add(entityIndex);
            world.GetPool<ComponentObjectAttributes>().Add(entityIndex);

            UpdateEntity(world, entityIndex, game, entityData);
        }
        
        private void UpdateEntity(EcsWorld world, int entityIndex, IGame game, JObject entityData)
        {
            world.GetPool<ComponentObjectId>().Get(entityIndex).Id = entityData.GetValue<int>("id");
            var typeId = entityData.GetValue<int>("tplId");
            world.GetPool<ComponentObjectType>().Get(entityIndex).TplId = typeId;

            // interactable
            UpdateInteractable(typeId, world, game, entityIndex);
            
            // attributes
            ref var attributesComponent = ref world.GetPool<ComponentObjectAttributes>().Get(entityIndex);
            var attributeArray = entityData.GetValue<JArray>("attrs");
            UpdateAttributes(attributeArray, attributesComponent.Attributes);
            _staticAttributes.ApplyAndUpdateAttributes(world, entityIndex, attributesComponent.Attributes);
        }
        
        private void UpdateInteractable(int tplid, EcsWorld world, IGame game, int entityIndex)
        {
            if (game.ServiceGameContent.ItemTypes.Items.ContainsKey(tplid))
            {
                world.GetPool<ComponentAttributeInteractable>().Add(entityIndex).Update(1);
            }
        }
    }
}
#endif