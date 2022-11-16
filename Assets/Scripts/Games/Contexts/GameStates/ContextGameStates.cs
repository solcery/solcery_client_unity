using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.Games.Contexts.GameStates.Actions;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Simulation.Game.State;
using Solcery.Services.LocalSimulation;
using UnityEngine;

namespace Solcery.Games.Contexts.GameStates
{
    public sealed class ContextGameStates : IContextGameStates
    {
        private sealed class AttrData
        {
            public string Key => _key;
            public int Value => _value;
            
            private readonly string _key;
            private readonly int _value;

            public static AttrData Create(string key, int value)
            {
                return new AttrData(key, value);
            }

            private AttrData(string key, int value)
            {
                _key = key;
                _value = value;
            }
        }
        
        private sealed class ObjectData
        {
            public int Id => _id;
            public int TplId => _tplId;
            public IReadOnlyDictionary<string, AttrData> Attrs => _attrs;

            private readonly int _id;
            private readonly int _tplId;
            private readonly Dictionary<string, AttrData> _attrs;

            public static ObjectData Create(int id, int tplId)
            {
                return new ObjectData(id, tplId);
            }

            private ObjectData(int id, int tplId)
            {
                _id = id;
                _tplId = tplId;
                _attrs = new Dictionary<string, AttrData>();
            }

            public void AddAttr(AttrData attrData)
            {
                _attrs.Add(attrData.Key, attrData);
            }
            
            public bool TryGetAttr(string key, out AttrData attrData)
            {
                return _attrs.TryGetValue(key, out attrData);
            }
        }

        private abstract class StateData { }
        
        private sealed class GameStateData : StateData
        {
            private readonly Dictionary<string, AttrData> _attrs;
            private readonly List<int> _deletedObjects;
            private readonly Dictionary<int, ObjectData> _objects;

            public static GameStateData Create()
            {
                return new GameStateData();
            }

            private GameStateData()
            {
                _attrs = new Dictionary<string, AttrData>();
                _deletedObjects = new List<int>();
                _objects = new Dictionary<int, ObjectData>();
            }

            private bool TryGetAttr(string key, out AttrData attrData)
            {
                return _attrs.TryGetValue(key, out attrData);
            }

            private bool TryGetObject(int objId, out ObjectData objectData)
            {
                return _objects.TryGetValue(objId, out objectData);
            }

            public void AddAttr(AttrData attrData)
            {
                _attrs.Add(attrData.Key, attrData);
            }

            public void AddDeletedObject(int objId)
            {
                _deletedObjects.Add(objId);
            }

            public void AddObject(ObjectData objectData)
            {
                _objects.Add(objectData.Id, objectData);
            }

            public JObject ToJson(GameStateData previewGameStateData)
            {
                var result = new JObject();

                {
                    var attrsArray = new JArray();
                    
                    foreach (var attrData in _attrs)
                    {
                        var key = attrData.Value.Key;
                        var value = attrData.Value.Value;

                        if (previewGameStateData != null)
                        {
                            if (previewGameStateData.TryGetAttr(key, out var previewAttrData)
                                && previewAttrData.Value == value)
                            {
                                continue;
                            }
                        }

                        attrsArray.Add(new JObject
                        {
                            ["key"] = new JValue(key),
                            ["value"] = new JValue(value)
                        });
                    }

                    result.Add("attrs", attrsArray);
                }

                {
                    var deletedObjectsArray = new JArray();

                    foreach (var objId in _deletedObjects)
                    {
                        deletedObjectsArray.Add(new JValue(objId));
                    }
                    
                    result.Add("deleted_objects", deletedObjectsArray);
                }

                {
                    var objectsArray = new JArray();

                    foreach (var objectData in _objects)
                    {
                        var obj = new JObject
                        {
                            {"id", new JValue(objectData.Value.Id)},
                            {"tplId", new JValue(objectData.Value.TplId)}
                        };

                        var attrsArray = new JArray();
                        var previewObjectData = previewGameStateData != null
                            ? previewGameStateData.TryGetObject(objectData.Value.Id, out var objData) ? objData : null
                            : null;

                        foreach (var attrData in objectData.Value.Attrs)    
                        {
                            var key = attrData.Value.Key;
                            var value = attrData.Value.Value;

                            if (previewObjectData != null 
                                && previewObjectData.TryGetAttr(key, out var previewAttrData)
                                && previewAttrData.Value == value)
                            {
                                continue;
                            }
                            
                            attrsArray.Add(new JObject
                            {
                                ["key"] = new JValue(key),
                                ["value"] = new JValue(value)
                            });
                        }
                        
                        obj.Add("attrs", attrsArray);

                        if (attrsArray.Count > 0)
                        {
                            objectsArray.Add(obj);
                        }
                    }
                    
                    result.Add("objects", objectsArray);
                }

                return result;
            }
        }

        public bool IsEmpty => true;

        private readonly EcsWorld _world;
        private readonly IServiceLocalSimulationApplyGameStateNew _applyGameStateNew;
        private readonly EcsFilter _filterGameAttributes;
        private readonly EcsFilter _filterEntities;
        private readonly EcsFilter _filterGameStateIndex;

        public static IContextGameStates Create(EcsWorld world, IServiceLocalSimulationApplyGameStateNew applyGameStateNew)
        {
            return new ContextGameStates(world, applyGameStateNew);
        }

        private ContextGameStates(EcsWorld world, IServiceLocalSimulationApplyGameStateNew applyGameStateNew)
        {
            _world = world;
            _applyGameStateNew = applyGameStateNew;
            _filterGameAttributes = world.Filter<ComponentGameAttributes>().End();
            _filterEntities = world.Filter<ComponentObjectTag>().Inc<ComponentObjectId>().Inc<ComponentObjectType>()
                .Inc<ComponentObjectAttributes>().End();
            _filterGameStateIndex = world.Filter<ComponentGameStateIndex>().End();
        }
        
        void IContextGameStates.PushGameState()
        {
            var gameStateValue = GameStateData.Create();
            
            // Game attributes
            {
                foreach (var gameAttributesEntityId in _filterGameAttributes)
                {
                    var attrs = _world.GetPool<ComponentGameAttributes>().Get(gameAttributesEntityId).Attributes;

                    foreach (var (key, value) in attrs)
                    {
                        gameStateValue.AddAttr(AttrData.Create(key, value.Current));
                    }

                    break;
                }
            }

            // Entities
            {
                var entityIdPool = _world.GetPool<ComponentObjectId>();
                var entityTypePool = _world.GetPool<ComponentObjectType>();
                var entityAttributesPool = _world.GetPool<ComponentObjectAttributes>();
                var entityDeletedPool = _world.GetPool<ComponentObjectDeletedTag>();
                foreach (var entityId in _filterEntities)
                {
                    if (entityDeletedPool.Has(entityId))
                    {
                        Debug.Log($"Discard object at destroy tag with entity id {entityId}");
                        gameStateValue.AddDeletedObject(entityIdPool.Get(entityId).Id);
                        continue;
                    }

                    var objData = ObjectData.Create(entityIdPool.Get(entityId).Id, entityTypePool.Get(entityId).TplId);

                    var attrs = entityAttributesPool.Get(entityId).Attributes;

                    foreach (var (key, value) in attrs)
                    {
                        if (value.Changed)
                        {
                            objData.AddAttr(AttrData.Create(key, value.Current));
                        }
                    }

                    gameStateValue.AddObject(objData);
                }
            }
            
            var gameState = new JObject
            {
                { "actions", new JArray() },
                {
                    "states", new JArray
                    {
                        new JObject
                        {
                            { "id", new JValue(GetGameStateIndex()) },
                            { "state_type", new JValue((int)ContextGameStateTypes.GameState) },
                            { "value", gameStateValue.ToJson(null) }
                        }
                    }
                }
            };
            _applyGameStateNew.ApplySimulatedGameStates(gameState);
            IncrementGameStateIndex();
        }

        void IContextGameStates.PushDelay(int msec)
        {
            var gameState = new JObject
            {
                { "actions", new JArray() },
                {
                    "states", new JArray
                    {
                        new JObject
                        {
                            { "id", new JValue(-1) },
                            { "state_type", new JValue((int)ContextGameStateTypes.Delay) },
                            { "value", new JObject
                                {
                                    { "delay", new JValue(msec) }
                                } 
                            }
                        }
                    }
                }
            };
            _applyGameStateNew.ApplySimulatedGameStates(gameState);
        }

        void IContextGameStates.PushStartTimer(int durationMsec, int targetObjectId)
        {
            var gameState = new JObject
            {
                { "actions", new JArray() },
                {
                    "states", new JArray
                    {
                        new JObject
                        {
                            { "id", new JValue(-1) },
                            { "state_type", new JValue((int)ContextGameStateTypes.Timer) },
                            { "value", new JObject
                                {
                                    { "start", new JValue(true) },
                                    { "duration", new JValue(durationMsec) },
                                    { "object_id", new JValue(targetObjectId) }
                                } 
                            }
                        }
                    }
                }
            };
            _applyGameStateNew.ApplySimulatedGameStates(gameState);
        }

        void IContextGameStates.PushStopTimer()
        {
            var gameState = new JObject
            {
                { "actions", new JArray() },
                {
                    "states", new JArray
                    {
                        new JObject
                        {
                            { "id", new JValue(-1) },
                            { "state_type", new JValue((int)ContextGameStateTypes.Timer) },
                            { "value", new JObject
                                {
                                    { "start", new JValue(false) },
                                    { "duration", new JValue(0) },
                                    { "object_id", new JValue(-1) }
                                } 
                            }
                        }
                    }
                }
            };
            _applyGameStateNew.ApplySimulatedGameStates(gameState);
        }

        void IContextGameStates.PushPlaySound(int soundId)
        {
            var gameState = new JObject
            {
                {
                    "actions", new JArray
                    {
                        new JObject
                        {
                            { "state_id", new JValue(GetGameStateIndex()) },
                            { "action_type", new JValue((int)ContextGameStateActionTypes.PlaySound) },
                            {
                                "value", new JObject
                                {
                                    { "sound_id", new JValue(soundId) }
                                }
                            }
                        }
                    }
                },
                { "states", new JArray() }
            };
            _applyGameStateNew.ApplySimulatedGameStates(gameState);
        }

        public bool TryGetGameState(int deltaTimeMsec, out JObject gameState)
        {
            gameState = new JObject();
            return true;
        }

        private int GetGameStateIndex()
        {
            if (_filterGameStateIndex.GetEntitiesCount() <= 0)
            {
                var entity = _world.NewEntity();
                _world.GetPool<ComponentGameStateIndex>().Add(entity).Index = 0;
                return 0;
            }

            foreach (var entity in _filterGameStateIndex)
            {
                return _world.GetPool<ComponentGameStateIndex>().Get(entity).Index;
            }

            return -1;
        }

        private void IncrementGameStateIndex()
        {
            if (_filterGameStateIndex.GetEntitiesCount() <= 0)
            {
                var entity = _world.NewEntity();
                _world.GetPool<ComponentGameStateIndex>().Add(entity).Index = 0;
                return;
            }
            
            foreach (var entity in _filterGameStateIndex)
            {
                var index = _world.GetPool<ComponentGameStateIndex>().Get(entity).Index;
                _world.GetPool<ComponentGameStateIndex>().Get(entity).Index = index + 1;
                return;
            }
        }
    }
}