using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models;
using Solcery.Models.Entities;
using Solcery.Models.Game;
using Solcery.Services.Transport;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Services.GameState
{
    public sealed class GameStateService : IGameStateService
    {
        private ITransportService _transportService;
        private IModel _model;
        private EcsFilter _filterGameAttributes;
        private EcsFilter _filterEntities;
        private readonly Queue<JObject> _queueGameStates;

        public static IGameStateService Create(ITransportService transportService, IModel model)
        {
            return new GameStateService(transportService, model);
        }

        private GameStateService(ITransportService transportService, IModel model)
        {
            _transportService = transportService;
            _model = model;
            _queueGameStates = new Queue<JObject>();
        }

        void IGameStateService.Init()
        {
            _filterGameAttributes = _model.World.Filter<ComponentGameAttributes>().End();
            _filterEntities = _model.World.Filter<ComponentEntityTag>().End();
            _transportService.EventReceivingGameState += OnEventReceivingGameState;
        }

        void IGameStateService.Update()
        {
            while (_queueGameStates.Count > 0)
            {
                ApplyGameState(_queueGameStates.Dequeue());
            }
        }

        void IGameStateService.Cleanup()
        {
            Cleanup();
        }

        void IGameStateService.Destroy()
        {
            Cleanup();
            _filterGameAttributes = null;
            _model = null;
            _transportService = null;
        }

        private void Cleanup()
        {
            _filterGameAttributes = null;
            _filterEntities = null;
            _transportService.EventReceivingGameState -= OnEventReceivingGameState;
            _queueGameStates.Clear();
        }
        
        private void OnEventReceivingGameState(JObject obj)
        {
            Debug.Log($"Game state\n {obj}");
            _queueGameStates.Enqueue(obj);
        }

        private void ApplyGameState(JObject obj)
        {
            // TODO: тут инитим гейм стейт, работаем с можелью и тд
            
            // Пропарсим игровые аттрибуты
            {
                var gameAttributeArray = obj.TryGetValue("attrs", out JArray attrs) ? attrs : null;
                if (_filterGameAttributes.GetEntitiesCount() <= 0)
                {
                    var entityIndex = _model.World.NewEntity();
                    ref var gameAttributesComponent =
                        ref _model.World.GetPool<ComponentGameAttributes>().Add(entityIndex);
                    gameAttributesComponent.Attributes = new Dictionary<string, int>(gameAttributeArray?.Count ?? 0);
                    UpdateAttributes(gameAttributeArray, gameAttributesComponent.Attributes);
                }
                else
                {
                    ref var gameAttributesComponent = ref _model.World.GetPool<ComponentGameAttributes>()
                        .Get(_filterGameAttributes.GetRawEntities()[0]);
                    UpdateAttributes(gameAttributeArray, gameAttributesComponent.Attributes);
                }
            }
            
            // Пропарсим игровые entity
            {
                var entityArray = obj.GetValue<JArray>("objects");
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
                    var entityId = _model.World.GetPool<ComponentEntityId>().Get(entityIndex).Id;

                    if (!entityHashMap.ContainsKey(entityId))
                    {
                        _model.World.DelEntity(entityIndex);
                        continue;
                    }

                    UpdateEntity(entityId, entityHashMap[entityId]);
                    entityHashMap.Remove(entityId);
                }

                foreach (var entityObject in entityHashMap)
                {
                    CreateEntity(entityObject.Value);
                }
                
                entityHashMap.Clear();
            }
        }

        private void UpdateAttributes(JArray gameAttributeArray, Dictionary<string, int> attributesHashMap)
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
                        attributesHashMap.Add(key, value);
                        continue;
                    }

                    attributesHashMap[key] = value;
                }
            }
        }

        private void CreateEntity(JObject entityData)
        {
            var entityIndex = _model.World.NewEntity();
            _model.World.GetPool<ComponentEntityTag>().Add(entityIndex);
            _model.World.GetPool<ComponentEntityId>().Add(entityIndex);
            _model.World.GetPool<ComponentEntityType>().Add(entityIndex);
            _model.World.GetPool<ComponentEntityAttributes>().Add(entityIndex).Attributes =
                new Dictionary<string, int>();
            
            UpdateEntity(entityIndex, entityData);
        }

        private void UpdateEntity(int entityIndex, JObject entityData)
        {
            _model.World.GetPool<ComponentEntityId>().Get(entityIndex).Id = entityData.GetValue<int>("id");
            _model.World.GetPool<ComponentEntityType>().Get(entityIndex).Type = entityData.GetValue<int>("tplId");
            
            ref var attributesComponent = ref _model.World.GetPool<ComponentEntityAttributes>().Get(entityIndex);
            UpdateAttributes(entityData.GetValue<JArray>("attrs"), attributesComponent.Attributes);
        }
    }
}