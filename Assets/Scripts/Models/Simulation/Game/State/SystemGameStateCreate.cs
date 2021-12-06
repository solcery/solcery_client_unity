using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Objects;
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
            private int _updateIteration;
            private readonly Dictionary<string, int> _oldGameAttributes;
            private readonly Dictionary<int, Dictionary<string, int>> _oldEntityAttributes;

            public static GameStateDiffLog Create()
            {
                return new GameStateDiffLog();
            }
            
            private GameStateDiffLog()
            {
                _updateIteration = 0;
                _oldGameAttributes = new Dictionary<string, int>();
                _oldEntityAttributes = new Dictionary<int, Dictionary<string, int>>();
            }

            public void PrintDiff(JObject gameState)
            {
                ++_updateIteration;
                var result = new JObject
                {
                    {"diff_iteration", new JValue(_updateIteration)}
                };
                
                if (gameState.TryGetValue("attrs", out JArray gameAttrsArray) 
                    && PrintGameAttrsDiff(gameAttrsArray, out var res1))
                {
                    result.Add("attrs", res1);
                }

                if (gameState.TryGetValue("objects", out JArray entitiesArray) 
                    && PrintEntitiesAttrDiff(entitiesArray, out var res2))
                {
                    result.Add("objects", res2);
                }
                
                Debug.Log(result.ToString(Formatting.Indented));
            }

            private bool PrintGameAttrsDiff(JArray gameAttrsArray, out JObject result)
            {
                result = new JObject();
                
                foreach (var attrToken in gameAttrsArray)
                {
                    if (PrintAttrDiff(_oldGameAttributes, attrToken, out var res))
                    {
                        result.Add(res.Item1, new JValue(res.Item2));
                    }
                }

                return result.Count > 0;
            }

            private bool PrintEntitiesAttrDiff(JArray entityArray, out JArray result)
            {
                result = new JArray();

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


                        var resArr = new JObject();

                        foreach (var attrToken in entityAttrsArray)
                        {
                            if (PrintAttrDiff(_oldEntityAttributes[id], attrToken, out var res))
                            {
                                resArr.Add(res.Item1, new JValue(res.Item2));
                            }
                        }

                        if (resArr.Count > 0)
                        {
                            var resObj = new JObject {{"id", new JValue(id)}};
                            result.Add(resObj);
                            resObj.Add("attrs", resArr);
                        }
                    }
                }

                return result.Count > 0;
            }

            private bool PrintAttrDiff(IDictionary<string, int> attrs, JToken attrToken, out Tuple<string, string> result)
            {
                result = null;
                
                if (attrToken is JObject attrObject 
                    && attrObject.TryGetValue("key", out string key)
                    && attrObject.TryGetValue("value", out int value))
                {
                    if (!attrs.ContainsKey(key) || attrs[key] != value)
                    {
                        if (!attrs.ContainsKey(key))
                        {
                            attrs.Add(key, value);
                        }
                        else
                        {
                            result = new Tuple<string, string>(key, $"{attrs[key]}->{value}");
                            attrs[key] = value;
                            return true;
                        }
                    }
                }
                
                return false;
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
            _filterEntities = world.Filter<ComponentObjectTag>().Inc<ComponentObjectId>().Inc<ComponentObjectType>()
                .Inc<ComponentObjectAttributes>().End();
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
            var entityIdPool = world.GetPool<ComponentObjectId>();
            var entityTypePool = world.GetPool<ComponentObjectType>();
            var entityAttributesPool = world.GetPool<ComponentObjectAttributes>();
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