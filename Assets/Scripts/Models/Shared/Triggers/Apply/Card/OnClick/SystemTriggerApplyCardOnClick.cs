using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games.Contexts;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types.OnClick;
using Solcery.Services.LocalSimulation;
using UnityEngine;

namespace Solcery.Models.Shared.Triggers.Apply.Card.OnClick
{
    // тут нужно разделитьы
    public interface ISystemTriggerApplyCardOnClick : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemTriggerApplyCardOnClick : ISystemTriggerApplyCardOnClick
    {
        private IServiceBricks _serviceBricks;
        private IServiceLocalSimulationApplyGameStateNew _applyGameState;
        private EcsFilter _filterTriggers;
        private EcsFilter _filterContext;
        private EcsFilter _filterEntityTypes;
        private EcsFilter _filterEntities;

        public static ISystemTriggerApplyCardOnClick Create(IServiceBricks serviceBricks, IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            return new SystemTriggerApplyCardOnClick(serviceBricks, applyGameState);
        }
        
        private SystemTriggerApplyCardOnClick(IServiceBricks serviceBricks, IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            _serviceBricks = serviceBricks;
            _applyGameState = applyGameState;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            
            _filterTriggers = world.Filter<ComponentTriggerTag>()
                .Inc<ComponentTriggerTargetObjectId>()
                .Inc<ComponentTriggerEntityCardTag>()
                .Inc<ComponentTriggerOnClickTag>()
                .End();

            _filterContext = world.Filter<ComponentContextObject>()
                .Inc<ComponentContextArgs>()
                .Inc<ComponentContextVars>()
                .End();

            _filterEntityTypes = world.Filter<ComponentObjectTypes>().End();

            _filterEntities = world.Filter<ComponentObjectTag>().Inc<ComponentObjectId>().Inc<ComponentObjectType>()
                .End();
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _serviceBricks = null;
            _filterTriggers = null;
            _filterContext = null;
            _filterEntityTypes = null;
            _filterEntities = null;
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var triggerTargetEntityIdPool = world.GetPool<ComponentTriggerTargetObjectId>();
            var entityIdPool = world.GetPool<ComponentObjectId>();
            var entityTypePool = world.GetPool<ComponentObjectType>();
            var entityTypesPool = world.GetPool<ComponentObjectTypes>();

            foreach (var triggerEntityId in _filterTriggers)
            {
                var targetObjectId = triggerTargetEntityIdPool.Get(triggerEntityId).TargetObjectId;

                foreach (var entityId in _filterEntities)
                {
                    if (entityIdPool.Get(entityId).Id != targetObjectId)
                    {
                        continue;
                    }

                    var entityType = entityTypePool.Get(entityId).Type;
                    var entityTypesId = -1;
                    foreach (var etid in _filterEntityTypes)
                    {
                        entityTypesId = etid;
                        break;
                    }
                
                    if (entityTypesId == -1 || 
                        !entityTypesPool.Get(entityTypesId).Types.TryGetValue(entityType, out var entityTypeData) 
                        || !entityTypeData.TryGetValue("action", out JObject brick))
                    {
                        continue;
                    }

                    InitContext(entityId, world);
                    
                    // TODO: fix it!!!
                    var context = CurrentContext.Create(world);
                    
                    Debug.Log($"Action brick execute status {_serviceBricks.ExecuteBrick(brick, context, 1)}");
                    
                    // TODO: fix it!!!
                    _applyGameState.ApplySimulatedGameStates(context.GameStates);
                    
                    DestroyContext(world);
                    
                    break;
                }
                
                world.DelEntity(triggerEntityId);
            }
        }

        private void InitContext(int targetEntityId, EcsWorld world)
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
            contextVarsPool.Add(contextEntityId);
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