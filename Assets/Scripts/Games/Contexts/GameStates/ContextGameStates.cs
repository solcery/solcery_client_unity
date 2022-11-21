using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.Games.Contexts.GameStates.Actions;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Objects;
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
        
        private sealed class PauseStateData : StateData
        {
            private readonly int _delayMSec;

            public static PauseStateData Create(int delayMSec)
            {
                return new PauseStateData(delayMSec);
            }

            private PauseStateData(int delayMSec)
            {
                _delayMSec = delayMSec;
            }

            public JObject ToJson()
            {
                return new JObject {{"delay", new JValue(_delayMSec)}};
            }
        }
        
        private sealed class TimerStateData : StateData
        {
            private readonly bool _isStart;
            private readonly int _durationMsec;
            private readonly int _targetObjectId;
            
            public static TimerStateData CreateStartTimer(int durationMsec, int targetObjectId)
            {
                return new TimerStateData(true, durationMsec, targetObjectId);
            }

            public static TimerStateData CreateStopTimer()
            {
                return new TimerStateData(false, 0, -1);
            }

            private TimerStateData(bool isStart, int durationMsec, int targetObjectId)
            {
                _isStart = isStart;
                _durationMsec = durationMsec;
                _targetObjectId = targetObjectId;
            }
            
            public JObject ToJson()
            {
                return new JObject
                {
                    {"start", new JValue(_isStart)},
                    {"duration", new JValue(_durationMsec)},
                    {"object_id", new JValue(_targetObjectId)}
                };
            }
        }

        public bool IsEmpty => _states.Count <= 0;

        private readonly EcsWorld _world;
        private readonly EcsFilter _filterGameAttributes;
        private readonly EcsFilter _filterEntities;
        private readonly List<StateData> _states;
        private readonly IContextGameStateActions _actions;

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
            _states = new List<StateData>();
            _actions = ContextGameStateActions.Create();
        }
        
        void IContextGameStates.PushGameState()
        {
            var gameState = GameStateData.Create();
            
            // Game attributes
            {
                foreach (var gameAttributesEntityId in _filterGameAttributes)
                {
                    var attrs = _world.GetPool<ComponentGameAttributes>().Get(gameAttributesEntityId).Attributes;

                    foreach (var (key, value) in attrs)
                    {
                        gameState.AddAttr(AttrData.Create(key, value.Current));
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
                        gameState.AddDeletedObject(entityIdPool.Get(entityId).Id);
                        continue;
                    }

                    var objData = ObjectData.Create(entityIdPool.Get(entityId).Id, entityTypePool.Get(entityId).TplId);

                    var attrs = entityAttributesPool.Get(entityId).Attributes;

                    foreach (var (key, value) in attrs)
                    {
                        objData.AddAttr(AttrData.Create(key, value.Current));
                    }

                    gameState.AddObject(objData);
                }
            }

            _states.Add(gameState);
            _actions.SetTargetStateId(_states.IndexOf(gameState));
            _actions.Push();
        }

        void IContextGameStates.PushDelay(int msec)
        {
            var delayState = PauseStateData.Create(msec);
            _states.Add(delayState);
        }

        void IContextGameStates.PushStartTimer(int durationMsec, int targetObjectId)
        {
            var timerStartState = TimerStateData.CreateStartTimer(durationMsec, targetObjectId);
            _states.Add(timerStartState);
        }

        void IContextGameStates.PushStopTimer()
        {
            var timerStopState = TimerStateData.CreateStopTimer();
            _states.Add(timerStopState);
        }

        void IContextGameStates.PushPlaySound(int soundId, int volume)
        {
            _actions.AddAction(ContextGameStateActionPlaySound.Create(soundId, volume));
        }

        public bool TryGetGameState(int deltaTimeMsec, out JObject gameState)
        {
            gameState = new JObject();
            var stateArray = new JArray();
            gameState.Add("actions", _actions.ToJson());
            gameState.Add("states", stateArray);
            GameStateData previewGameStateData = null;
            
            foreach (var state in _states)
            {
                switch (state)
                {
                    case PauseStateData psd:
                        stateArray.Add(CreateState(_states.IndexOf(state), ContextGameStateTypes.Delay, psd.ToJson()));
                        break;
                    
                    case TimerStateData tsd:
                        stateArray.Add(CreateState(_states.IndexOf(state), ContextGameStateTypes.Timer, tsd.ToJson()));
                        break;
                    
                    case GameStateData gsd:
                        stateArray.Add(CreateState(_states.IndexOf(state), ContextGameStateTypes.GameState, gsd.ToJson(previewGameStateData)));
                        previewGameStateData = gsd;
                        break;
                }
            }

            _states.Clear();
            _actions.Cleanup();

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