using System.Collections.Generic;
using System.Linq;
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
        public bool IsEmpty => _states.Count <= 0;

        private readonly EcsWorld _world;
        private readonly EcsFilter _filterGameAttributes;
        private readonly EcsFilter _filterEntities;

        private int _gameStateInPackage;
        private readonly List<JObject> _states;

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
            _gameStateInPackage = 0;
            _states = new List<JObject>();
        }
        
        public void PushGameState()
        {
            var gameState = new JObject();
            
            // Game attributes
            foreach (var gameAttributesEntityId in _filterGameAttributes)
            {
                if (!TryCreateAttributeArray(_gameStateInPackage == 0, _world.GetPool<ComponentGameAttributes>().Get(gameAttributesEntityId).Attributes, out var gameAttributesArray))
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
                if (!TryCreateAttributeArray(_gameStateInPackage == 0, entityAttributesPool.Get(entityId).Attributes, out var attributesArray) 
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
            _states.Add(CreateState(_states.Count, ContextGameStateTypes.GameState, gameState));
            ++_gameStateInPackage;
        }

        public void PushDelay(int msec)
        {
            var delayState = new JObject {{"delay", new JValue(msec)}};
            _states.Add(CreateState(_states.Count, ContextGameStateTypes.Delay, delayState));
        }

        public bool TryGetGameState(int deltaTimeMsec, out JObject gameState)
        {
            _gameStateInPackage = 0;

            gameState = new JObject();
            var stateArray = new JArray();
            gameState.Add("states", stateArray);
            
            foreach (var state in _states)
            {
                stateArray.Add(state);
            }

            return true;
        }
        
        private static bool TryCreateAttributeArray(bool firstGameState, Dictionary<string, IAttributeValue> attributesHashMap, out JArray attributeArray)
        {
            if (attributesHashMap is not {Count: > 0} 
                || !firstGameState && attributesHashMap.Count(o=>o.Value.Changed) <= 0)
            {
                attributeArray = null;
                return false;
            }

            attributeArray = new JArray();
            foreach (var (key, value) in attributesHashMap)
            {
                if (!firstGameState && !value.Changed)
                {
                    continue;
                }
                
                attributeArray.Add(new JObject
                {
                    ["key"] = new JValue(key),
                    ["value"] = new JValue(value.Current)
                });
            }
            
            return true;
        }

        private static JObject CreateState(int id, ContextGameStateTypes type, JToken value)
        {
            return new JObject
            {
                {"id", new JValue(id)},
                {"state_type", new JValue((int) type)},
                {"value", value}
            };
        }
    }
}