using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games.Contexts;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types.OnClick;
using Solcery.Services.LocalSimulation;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Shared.Triggers.Apply.Card.OnClick
{
    public interface ISystemTriggerApplyCardOnRightClick : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }
    
    public class SystemTriggerApplyCardOnRightClick : ISystemTriggerApplyCardOnRightClick
    {
        private IServiceBricks _serviceBricks;
        private readonly IServiceLocalSimulationApplyGameStateNew _applyGameState;
        private EcsFilter _filterTriggers;
        private EcsFilter _filterEntityTypes;
        private EcsFilter _filterEntities;
        
        public static ISystemTriggerApplyCardOnRightClick Create(IServiceBricks serviceBricks, IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            return new SystemTriggerApplyCardOnRightClick(serviceBricks, applyGameState);
        }
        
        private SystemTriggerApplyCardOnRightClick(IServiceBricks serviceBricks, IServiceLocalSimulationApplyGameStateNew applyGameState)
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
                .Inc<ComponentTriggerOnRightClickTag>()
                .End();

            _filterEntityTypes = world.Filter<ComponentObjectTypes>().End();

            _filterEntities = world.Filter<ComponentObjectTag>().Inc<ComponentObjectId>().Inc<ComponentObjectType>()
                .End();
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _serviceBricks = null;
            _filterTriggers = null;
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

                    var entityType = entityTypePool.Get(entityId).TplId;
                    var entityTypesId = -1;
                    foreach (var etid in _filterEntityTypes)
                    {
                        entityTypesId = etid;
                        break;
                    }

                    if (entityTypesId == -1 ||
                        !entityTypesPool.Get(entityTypesId).Types.TryGetValue(entityType, out var entityTypeData)
                        || !entityTypeData.TryGetValue("action_on_right_click", out JObject brick))
                    {
                        Debug.Log("Can't find action_on_right_click!");
                        continue;
                    }

                    //InitContext(entityId, world);

                    // TODO: fix it!!!
                    var context = CurrentContext.Create(world);
                    context.Object.Push(entityId);

                    Debug.Log($"Action brick execute status {_serviceBricks.ExecuteBrick(brick, context, 1)}");

                    // TODO: fix it!!!
                    _applyGameState.ApplySimulatedGameStates(context.GameStates);
                    CurrentContext.Destroy(world, context);

                    //DestroyContext(world);

                    break;
                }

                world.DelEntity(triggerEntityId);
            }
        }
    }
}