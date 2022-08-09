using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Games.States.New.States;
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
        private IStaticAttributes _staticAttributes;
        
        public static ISystemGameStateUpdate Create(IGame game)
        {
            return new SystemGameStateUpdate(game);
        }

        private SystemGameStateUpdate(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterGameAttributes = world.Filter<ComponentGameAttributes>().End();
            _filterEntities = world.Filter<ComponentObjectTag>().End();
            _staticAttributes = StaticAttributes.Create();
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeHighlighted.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeInteractable.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeNumber.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributePlace.Create());
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var us = _game.UpdateStateQueue.CurrentState;

            if (us is not UpdateGameState ugs)
            {
                return;
            }

            var gameStateJson = ugs.GameState;

            var world = systems.GetWorld();
            var game = systems.GetShared<IGame>();
            
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
            
            // New game state method
            {
                // Update game entity
                var destroyHash = new HashSet<int>();
                if (gameStateJson.TryGetValue("deleted_objects", out JArray destroyArray))
                {
                    foreach (var objectIdToken in destroyArray)
                    {
                        destroyHash.Add(objectIdToken.Value<int>());
                    }
                }

                var updateObjects = new Dictionary<int, JObject>();
                if (gameStateJson.TryGetValue("objects", out JArray objectArray))
                {
                    foreach (var objectToken in objectArray)
                    {
                        if (objectToken is JObject objectObject
                            && objectObject.TryGetValue("id", out int id)
                            && !updateObjects.ContainsKey(id))
                        {
                            updateObjects.Add(id, objectObject);
                        }
                    }
                }

                var poolObjectId = world.GetPool<ComponentObjectId>();
                foreach (var entityId in _filterEntities)
                {
                    var objectId = poolObjectId.Get(entityId).Id;

                    if (destroyHash.Contains(objectId))
                    {
                        destroyHash.Remove(objectId);
                        world.DelEntity(entityId);
                        continue;
                    }

                    if (updateObjects.TryGetValue(objectId, out var objectData))
                    {
                        updateObjects.Remove(objectId);
                        UpdateEntity(world, entityId, game, objectData);
                    }
                }

                foreach (var updateObject in updateObjects)
                {
                    CreateEntity(world, game, updateObject.Value);
                }
                
                updateObjects.Clear();
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
        
        private void UpdateInteractable(int tplid, EcsWorld world, IGame game, int entityIndex)
        {
            if (game.ServiceGameContent.ItemTypes.Items.ContainsKey(tplid))
            {
                if (!world.GetPool<ComponentAttributeInteractable>().Has(entityIndex))
                {
                    world.GetPool<ComponentAttributeInteractable>().Add(entityIndex).Update(1);
                }
            }
        }

        private void CreateEntity(EcsWorld world, IGame game, JObject entityData)
        {
            var entityIndex = world.NewEntity();
            world.GetPool<ComponentObjectTag>().Add(entityIndex);
            world.GetPool<ComponentObjectId>().Add(entityIndex);
            world.GetPool<ComponentObjectType>().Add(entityIndex);
            world.GetPool<ComponentObjectAttributes>().Add(entityIndex).Attributes =
                new Dictionary<string, IAttributeValue>();

            UpdateEntity(world, entityIndex, game, entityData);
        }

        private void RemoveAllEclipseCardComponents(EcsWorld world, int entityIndex)
        {
            var tagPool = world.GetPool<ComponentEclipseCardTag>();
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var eventTagPool = world.GetPool<ComponentEclipseCardEventTag>();
            var creatureTagPool = world.GetPool<ComponentEclipseCardCreatureTag>();
            var buildingTagPool = world.GetPool<ComponentEclipseCardBuildingTag>();
            var eclipseTagPool = world.GetPool<ComponentEclipseCardEclipseTag>();
            var nftTagPool = world.GetPool<ComponentEclipseCardNftTag>();
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
            
            if (eclipseTagPool.Has(entityIndex))
            {
                eclipseTagPool.Del(entityIndex);
            }
            
            if (nftTagPool.Has(entityIndex))
            {
                nftTagPool.Del(entityIndex);
            }
            
            if (tokenTagPool.Has(entityIndex))
            {
                tokenTagPool.Del(entityIndex);
            }
        }

        private void UpdateEclipseCardComponents(EcsWorld world, int entityIndex, IGame game, JObject entityData)
        {
            RemoveAllEclipseCardComponents(world, entityIndex);
            
            var tagPool = world.GetPool<ComponentEclipseCardTag>();
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var eventTagPool = world.GetPool<ComponentEclipseCardEventTag>();
            var creatureTagPool = world.GetPool<ComponentEclipseCardCreatureTag>();
            var buildingTagPool = world.GetPool<ComponentEclipseCardBuildingTag>();
            var eclipseTagPool = world.GetPool<ComponentEclipseCardEclipseTag>();
            var nftTagPool = world.GetPool<ComponentEclipseCardNftTag>();
            var tokenTagPool = world.GetPool<ComponentEclipseTokenTag>();

            var objectId = entityData.GetValue<int>("id");
            var tplid = entityData.GetValue<int>("tplId");
            if (game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid))
            {
                if (itemType.TryGetValue(out var valueToken, GameJsonKeys.CardType, objectId)
                    && valueToken.TryGetEnum(out EclipseCardTypes eclipseCardType))
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
                        
                        case EclipseCardTypes.Eclipse:
                            eclipseTagPool.Add(entityIndex);
                            break;
                        
                        case EclipseCardTypes.Nft:
                            nftTagPool.Add(entityIndex);
                            break;
                    
                        case EclipseCardTypes.Token:
                            tokenTagPool.Add(entityIndex);
                            break;
                    }
                }
            }
        }

        private void UpdateEntity(EcsWorld world, int entityIndex, IGame game, JObject entityData)
        {
            // Eclipse card support
            UpdateEclipseCardComponents(world, entityIndex, game, entityData);
            
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

        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            _staticAttributes.Cleanup();
        }
    }
}