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
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Simulation.Game.State
{
    public interface ISystemGameStateInitial : IEcsInitSystem { }

    public sealed class SystemGameStateInitial : ISystemGameStateInitial
    {
        private JObject _initialGameState;
        private EcsFilter _filterObjectIdHash;
        private IStaticAttributes _staticAttributes;
        
        public static ISystemGameStateInitial Create(JObject initialGameState)
        {
            return new SystemGameStateInitial(initialGameState);
        }
        
        private SystemGameStateInitial(JObject initialGameState)
        {
            _initialGameState = initialGameState;
        }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            if (_initialGameState == null)
            {
                return;
            }
            
            _staticAttributes = StaticAttributes.Create();
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeHighlighted.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeInteractable.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributePlace.Create());

            var game = systems.GetShared<IGame>();
            var world = systems.GetWorld();
            _filterObjectIdHash = world.Filter<ComponentObjectIdHash>().End();
            var objectIdHashPool = world.GetPool<ComponentObjectIdHash>();

            foreach (var oih in _filterObjectIdHash)
            {
                world.DelEntity(oih);
            }

            var objectIdHashPoolEntityId = world.NewEntity();
            var objectIdHashes = objectIdHashPool.Add(objectIdHashPoolEntityId).ObjectIdHashes;
            
            // Update game attributes
            var gameAttributeArray = _initialGameState.TryGetValue("attrs", out JArray attrs) ? attrs : null;
            var entityIndex = world.NewEntity();
            ref var gameAttributesComponent =
                ref world.GetPool<ComponentGameAttributes>().Add(entityIndex);
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
            _initialGameState = null;
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