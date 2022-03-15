using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games.Contexts;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types.OnDrop;
using Solcery.Services.LocalSimulation;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Shared.Triggers.Apply.Card.OnDrop
{
    public interface ISystemTriggerApplyCardOnDrop : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemTriggerApplyCardOnDrop : ISystemTriggerApplyCardOnDrop
    {
        private IServiceBricks _serviceBricks;
        private IServiceLocalSimulationApplyGameStateNew _applyGameState;
        private EcsFilter _filterTriggers;
        private EcsFilter _filterObjects;
        private EcsFilter _filterObjectTypes;
        private EcsFilter _filterContext;

        public static ISystemTriggerApplyCardOnDrop Create(IServiceBricks serviceBricks, IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            return new SystemTriggerApplyCardOnDrop(serviceBricks, applyGameState);
        }

        private SystemTriggerApplyCardOnDrop(IServiceBricks serviceBricks, IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            _serviceBricks = serviceBricks;
            _applyGameState = applyGameState;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            
            _filterTriggers = world.Filter<ComponentTriggerTag>()
                .Inc<ComponentTriggerTargetObjectId>()
                .Inc<ComponentTriggerTargetPlaceId>()
                .Inc<ComponentTriggerEntityCardTag>()
                .Inc<ComponentTriggerOnDropTag>()
                .End();
            
            _filterObjects = world.Filter<ComponentObjectTag>()
                .Inc<ComponentObjectId>()
                .Inc<ComponentObjectType>()
                .End();
            
            _filterObjectTypes = world.Filter<ComponentObjectTypes>()
                .End();
            
            _filterContext = world.Filter<ComponentContextObject>()
                .Inc<ComponentContextArgs>()
                .Inc<ComponentContextVars>()
                .End();
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _serviceBricks = null;
            _filterTriggers = null;
            _filterObjects = null;
            _filterObjectTypes = null;
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var targetObjectIdPool = world.GetPool<ComponentTriggerTargetObjectId>();
            var targetPlaceIdPool = world.GetPool<ComponentTriggerTargetPlaceId>();
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var objectTypesPool = world.GetPool<ComponentObjectTypes>();
            
            foreach (var triggerEntityId in _filterTriggers)
            {
                var targetObjectId = targetObjectIdPool.Get(triggerEntityId).TargetObjectId;
                var targetPlaceId = targetPlaceIdPool.Get(triggerEntityId).TargetPlaceId;

                foreach (var objectEntityId in _filterObjects)
                {
                    if (objectIdPool.Get(objectEntityId).Id != targetObjectId)
                    {
                        continue;
                    }

                    var objectTypeId = objectTypePool.Get(objectEntityId).Type;

                    foreach (var objectTypeEntityId in _filterObjectTypes)
                    {
                        if (objectTypesPool.Get(objectTypeEntityId).Types.TryGetValue(objectTypeId, out var objectTypeData) 
                            && objectTypeData.TryGetValue("action", out JObject brick))
                        {
                            InitContext(objectEntityId, world, targetPlaceId);
                            
                            // TODO: fix it!!!
                            var context = CurrentContext.Create(world);
                            
                            Debug.Log($"Action brick execute status {_serviceBricks.ExecuteBrick(brick, context, 1)}");
                            
                            // TODO: fix it!!!
                            _applyGameState.ApplySimulatedGameStates(context.GameStates);
                            
                            
                            DestroyContext(world);
                        }
                        
                        break;
                    }
                    
                    break;
                }
                
                world.DelEntity(triggerEntityId);
            }
        }
        
        private void InitContext(int targetEntityId, EcsWorld world, int targetPlaceId)
        {
            var contextObjectPool = world.GetPool<ComponentContextObject>();
            var contextArgsPool = world.GetPool<ComponentContextArgs>();
            var contextVarsPool = world.GetPool<ComponentContextVars>();

            foreach (var entityId in _filterContext)
            {
                world.DelEntity(entityId);
            }

            var contextEntityId = world.NewEntity();
            contextObjectPool.Add(contextEntityId).Push(targetEntityId);
            contextArgsPool.Add(contextEntityId);
            contextVarsPool.Add(contextEntityId).Set("target_place", targetPlaceId);
        }
        
        private void DestroyContext(EcsWorld world)
        {
            foreach (var entityId in _filterContext)
            {
                world.DelEntity(entityId);
            }
        }
    }
}