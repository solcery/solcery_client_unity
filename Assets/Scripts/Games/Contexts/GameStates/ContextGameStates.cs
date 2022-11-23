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
            var gameStateValue = new JObject();
            
            // Game attributes
            {
                var attrsArray = new JArray();
                foreach (var gameAttributesEntityId in _filterGameAttributes)
                {
                    var attrs = _world.GetPool<ComponentGameAttributes>().Get(gameAttributesEntityId).Attributes;

                    foreach (var (key, value) in attrs)
                    {
                        if (value.Changed)
                        {
                            attrsArray.Add(new JObject
                            {
                                {"key", new JValue(key)},
                                {"value", new JValue(value.Current)}
                            });
                        }
                    }

                    break;
                }
                gameStateValue.Add("attrs", attrsArray);
            }

            // Entities
            {
                var entityIdPool = _world.GetPool<ComponentObjectId>();
                var entityTypePool = _world.GetPool<ComponentObjectType>();
                var entityAttributesPool = _world.GetPool<ComponentObjectAttributes>();
                var entityDeletedPool = _world.GetPool<ComponentObjectDeletedTag>();

                var deletedObjectsArray = new JArray();
                var objectsArray = new JArray();
                
                foreach (var entityId in _filterEntities)
                {
                    if (entityDeletedPool.Has(entityId))
                    {
                        Debug.Log($"Discard object at destroy tag with entity id {entityId}");
                        deletedObjectsArray.Add(new JValue(entityIdPool.Get(entityId).Id));
                        continue;
                    }

                    var changed = entityTypePool.Get(entityId).Changed;

                    var attrs = entityAttributesPool.Get(entityId).Attributes;
                    var attrsArray = new JArray();
                    foreach (var (key, value) in attrs)
                    {
                        if (value.Changed)
                        {
                            changed = true;
                            attrsArray.Add(new JObject
                            {
                                {"key", new JValue(key)},
                                {"value", new JValue(value.Current)}
                            });
                        }
                    }

                    if (changed)
                    {
                        objectsArray.Add(new JObject
                        {
                            {"id", new JValue(entityIdPool.Get(entityId).Id)},
                            {"tplId", new JValue(entityTypePool.Get(entityId).TplId)},
                            {"attrs", attrsArray}
                        });
                    }
                }
                
                gameStateValue.Add("deleted_objects", deletedObjectsArray);
                gameStateValue.Add("objects", objectsArray);
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
                            { "value", gameStateValue }
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

        void IContextGameStates.PushPlaySound(int soundId, int volume)
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
                                    { "sound_id", new JValue(soundId) },
                                    { "volume", new JValue(volume) }
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