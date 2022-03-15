using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Objects;

namespace Solcery.Games.Contexts.GameStates
{
    public sealed class ContextGameStates : IContextGameStates
    {
        public bool IsEmpty => _gameStates.Count <= 0;

        private readonly EcsWorld _world;
        private readonly EcsFilter _filterGameAttributes;
        private readonly EcsFilter _filterEntities;
        private readonly Queue<ContextGameState> _gameStates;

        public static IContextGameStates Create(EcsWorld world)
        {
            return new ContextGameStates(world);
        }

        private ContextGameStates(EcsWorld world)
        {
            _world = world;
            _filterGameAttributes = world.Filter<ComponentGameAttributes>().End();
            _filterEntities = world.Filter<ComponentObjectTag>().Inc<ComponentObjectId>().Inc<ComponentObjectType>()
                .Inc<ComponentObjectAttributes>().End();
            _gameStates = new Queue<ContextGameState>(10);
        }
        
        public void PushGameState()
        {
            var gameState = new JObject();
            
            // Game attributes
            foreach (var gameAttributesEntityId in _filterGameAttributes)
            {
                if (!TryCreateAttributeArray(_world.GetPool<ComponentGameAttributes>().Get(gameAttributesEntityId).Attributes, out var gameAttributesArray))
                {
                    gameAttributesArray = new JArray();
                }
                
                gameState.Add("attrs", gameAttributesArray);
                
                break;
            }
            
            // Entities
            var entityArray = new JArray();
            var entityIdPool = _world.GetPool<ComponentObjectId>();
            var entityTypePool = _world.GetPool<ComponentObjectType>();
            var entityAttributesPool = _world.GetPool<ComponentObjectAttributes>();
            var entityDeletedPool = _world.GetPool<ComponentObjectDeletedTag>();
            foreach (var entityId in _filterEntities)
            {
                if (!TryCreateAttributeArray(entityAttributesPool.Get(entityId).Attributes, out var attributesArray) 
                    || entityDeletedPool.Has(entityId))
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
            _gameStates.Enqueue(ContextGameStateData.Create(gameState));
        }

        public void PushDelay(int msec)
        {
            _gameStates.Enqueue(ContextGameStateDelay.Create(msec));
        }

        public bool TryGetGameState(int deltaTimeMsec, out JObject gameState)
        {
            if (_gameStates.TryPeek(out var gs))
            {
                if (gs is ContextGameStateDelay gsd 
                    && gsd.CanDestroy(deltaTimeMsec))
                {
                    _gameStates.Dequeue();
                }

                if (gs is ContextGameStateData gsdt)
                {
                    _gameStates.Dequeue();
                    gameState = gsdt.GameStateData;
                    return true;
                }
            }

            gameState = null;
            return false;
        }
        
        private bool TryCreateAttributeArray(Dictionary<string, IAttributeValue> attributesHashMap, out JArray attributeArray)
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
                    ["value"] = new JValue(attribute.Value.Current)
                });
            }
            
            return true;
        }
    }
}