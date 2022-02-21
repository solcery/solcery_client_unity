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
using Solcery.Models.Shared.Game.StaticAttributes.Number;
using Solcery.Models.Shared.Game.StaticAttributes.Place;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
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
            _filterEntities = world.Filter<ComponentObjectTag>().End();
            _filterEntityTypes = world.Filter<ComponentObjectTypes>().End();
            _staticAttributes = StaticAttributes.Create();
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeHighlighted.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeInteractable.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeNumber.Create());
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
                var entityId = world.GetPool<ComponentObjectId>().Get(entityIndex).Id;

                if (!entityHashMap.ContainsKey(entityId))
                {
                    world.DelEntity(entityIndex);
                    continue;
                }

                UpdateEntity(world, entityIndex, entityHashMap[entityId]);
                entityHashMap.Remove(entityId);
            }

            foreach (var entityObject in entityHashMap)
            {
                CreateEntity(world, entityObject.Value);
            }
                
            entityHashMap.Clear();
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
        
        private void UpdateInteractable(int typeId, EcsWorld world, int entityIndex)
        {
            foreach (var uniqEntityTypes in _filterEntityTypes)
            {
                ref var types = ref world.GetPool<ComponentObjectTypes>().Get(uniqEntityTypes);
                if (types.Types.ContainsKey(typeId))
                {
                    if (!world.GetPool<ComponentAttributeInteractable>().Has(entityIndex))
                    {
                        world.GetPool<ComponentAttributeInteractable>().Add(entityIndex).Update(1);
                    }
                }
            }
        }

        private void CreateEntity(EcsWorld world, JObject entityData)
        {
            var entityIndex = world.NewEntity();
            world.GetPool<ComponentObjectTag>().Add(entityIndex);
            world.GetPool<ComponentObjectId>().Add(entityIndex);
            world.GetPool<ComponentObjectType>().Add(entityIndex);
            world.GetPool<ComponentObjectAttributes>().Add(entityIndex).Attributes =
                new Dictionary<string, IAttributeValue>();

            UpdateEntity(world, entityIndex, entityData);
        }

        private void RemoveAllEclipseCardComponents(EcsWorld world, int entityIndex)
        {
            var tagPool = world.GetPool<ComponentEclipseCardTag>();
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var eventTagPool = world.GetPool<ComponentEclipseCardEventTag>();
            var creatureTagPool = world.GetPool<ComponentEclipseCardCreatureTag>();
            var buildingTagPool = world.GetPool<ComponentEclipseCardBuildingTag>();
            var tokenTagPool = world.GetPool<ComponentEclipseTokenTag>();

            if (tagPool.Has(entityIndex))
            {
                tagPool.Del(entityIndex);
            }
            
            if (eclipseCartTypePool.Has(entityIndex))
            {
                eclipseCartTypePool.Del(entityIndex);
            }
            
            if (eventTagPool.Has(entityIndex))
            {
                eventTagPool.Del(entityIndex);
            }
            
            if (creatureTagPool.Has(entityIndex))
            {
                creatureTagPool.Del(entityIndex);
            }
            
            if (buildingTagPool.Has(entityIndex))
            {
                buildingTagPool.Del(entityIndex);
            }
            
            if (tokenTagPool.Has(entityIndex))
            {
                tokenTagPool.Del(entityIndex);
            }
        }

        private void UpdateEclipseCardComponents(EcsWorld world, int entityIndex, JObject entityData)
        {
            RemoveAllEclipseCardComponents(world, entityIndex);
            
            var tagPool = world.GetPool<ComponentEclipseCardTag>();
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var eventTagPool = world.GetPool<ComponentEclipseCardEventTag>();
            var creatureTagPool = world.GetPool<ComponentEclipseCardCreatureTag>();
            var buildingTagPool = world.GetPool<ComponentEclipseCardBuildingTag>();
            var tokenTagPool = world.GetPool<ComponentEclipseTokenTag>();

            var typeId = entityData.GetValue<int>("tplId");
            foreach (var filterEntityTypeId in _filterEntityTypes)
            {
                ref var entityTypesComponent = ref world.GetPool<ComponentObjectTypes>().Get(filterEntityTypeId);
                if (entityTypesComponent.Types.TryGetValue(typeId, out var entityTypeData))
                {
                    if (entityTypeData.TryGetEnum("type", out EclipseCardTypes eclipseCardType))
                    {
                        tagPool.Add(entityIndex);
                        eclipseCartTypePool.Add(entityIndex).CardType = eclipseCardType;
                
                        switch (eclipseCardType)
                        {
                            case EclipseCardTypes.Event:
                                eventTagPool.Add(entityIndex);
                                break;
                    
                            case EclipseCardTypes.Creature:
                                creatureTagPool.Add(entityIndex);
                                break;
                    
                            case EclipseCardTypes.Building:
                                buildingTagPool.Add(entityIndex);
                                break;
                    
                            case EclipseCardTypes.Token:
                                tokenTagPool.Add(entityIndex);
                                break;
                        }
                    }
                }
            }
        }

        private void UpdateEntity(EcsWorld world, int entityIndex, JObject entityData)
        {
            // Eclipse card support
            UpdateEclipseCardComponents(world, entityIndex, entityData);
            
            world.GetPool<ComponentObjectId>().Get(entityIndex).Id = entityData.GetValue<int>("id");
            var typeId = entityData.GetValue<int>("tplId");
            world.GetPool<ComponentObjectType>().Get(entityIndex).Type = typeId;

            // interactable
            UpdateInteractable(typeId, world, entityIndex);
            
            // attributes
            ref var attributesComponent = ref world.GetPool<ComponentObjectAttributes>().Get(entityIndex);
            var attributeArray = entityData.GetValue<JArray>("attrs");
            UpdateAttributes(attributeArray, attributesComponent.Attributes);
            _staticAttributes.ApplyAndUpdateAttributes(world, entityIndex, attributesComponent.Attributes);
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _staticAttributes.Cleanup();
        }
    }
}