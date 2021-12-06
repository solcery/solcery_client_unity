using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Entities;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Services.LocalSimulation;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Simulation.Game.State
{
    public interface ISystemGameStateCreate : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemGameStateCreate : ISystemGameStateCreate
    {
        private sealed class GameStateDiffLog
        {
            private bool _isInitFirstGameState;
            private readonly Dictionary<string, int> _oldGameAttributes;
            private readonly Dictionary<int, Dictionary<string, int>> _oldEntityAttributes;

            public static GameStateDiffLog Create()
            {
                return new GameStateDiffLog();
            }
            
            private GameStateDiffLog()
            {
                _oldGameAttributes = new Dictionary<string, int>();
                _oldEntityAttributes = new Dictionary<int, Dictionary<string, int>>();
            }

            public void PrintDiff(JObject gameState)
            {
                Debug.Log("======[Game state diff]======");

                if (gameState.TryGetValue("attrs", out JArray gameAttrsArray))
                {
                    PrintGameAttrsDiff(gameAttrsArray);
                }

                if (gameState.TryGetValue("objects", out JArray entitiesArray))
                {
                    PrintEntitiesAttrDiff(entitiesArray);
                }

                Debug.Log("============[End]============");
            }

            private void PrintGameAttrsDiff(JArray gameAttrsArray)
            {
                Debug.Log("=========[Game attrs]========");
                foreach (var attrToken in gameAttrsArray)
                {
                    PrintAttrDiff(_oldGameAttributes, attrToken);
                }
                Debug.Log("============[End]============");
            }

            private void PrintEntitiesAttrDiff(JArray entityArray)
            {
                Debug.Log("==========[Entities]=========");
                foreach (var entityAttrToken in entityArray)
                {
                    if (entityAttrToken is JObject entityAttrObject 
                        && entityAttrObject.TryGetValue("id", out int id)
                        && entityAttrObject.TryGetValue("attrs", out JArray entityAttrsArray))
                    {
                        if (!_oldEntityAttributes.ContainsKey(id))
                        {
                            _oldEntityAttributes.Add(id, new Dictionary<string, int>());
                        }

                        foreach (var attrToken in entityAttrsArray)
                        {
                            PrintAttrDiff(_oldEntityAttributes[id], attrToken);
                        }
                    }
                }
                Debug.Log("============[End]============");
            }

            private void PrintAttrDiff(Dictionary<string, int> attrs, JToken attrToken)
            {
                if (attrToken is JObject attrObject 
                    && attrObject.TryGetValue("key", out string key)
                    && attrObject.TryGetValue("value", out int value))
                {
                    if (!attrs.ContainsKey(key) || _oldGameAttributes[key] != value)
                    {
                        if (!attrs.ContainsKey(key))
                        {
                            attrs.Add(key, value);
                        }
                        else
                        {
                            Debug.Log($"Attr {key} {attrs[key]}->{value}");
                            attrs[key] = value;
                        }
                    }
                }
            }
        }
        
        private IServiceLocalSimulationApplyGameState _applyGameState;
        private EcsFilter _filterGameAttributes;
        private EcsFilter _filterEntities;

        private GameStateDiffLog _diff;

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
            _diff = GameStateDiffLog.Create();
            
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
            
            _diff.PrintDiff(gameState);
            
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