using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games.Contexts;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types.OnDrop;
using Solcery.Services.LocalSimulation;
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
        private EcsFilter _filterDragDropParameters;

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
            
            _filterDragDropParameters = world.Filter<ComponentDragDropParametersTag>()
                .Inc<ComponentDragDropParametersId>()
                .Inc<ComponentDragDropParametersAction>()
                .End();
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _serviceBricks = null;
            _filterTriggers = null;
            _filterObjects = null;
            _filterDragDropParameters = null;
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var targetObjectIdPool = world.GetPool<ComponentTriggerTargetObjectId>();
            var targetDragDropIdPool = world.GetPool<ComponentTriggerTargetDragDropId>();
            var targetPlaceIdPool = world.GetPool<ComponentTriggerTargetPlaceId>();
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var dragDropIdPool = world.GetPool<ComponentDragDropParametersId>();
            var dragDropActionPool = world.GetPool<ComponentDragDropParametersAction>();
            
            foreach (var triggerEntityId in _filterTriggers)
            {
                var targetObjectId = targetObjectIdPool.Get(triggerEntityId).TargetObjectId;
                var targetPlaceId = targetPlaceIdPool.Get(triggerEntityId).TargetPlaceId;
                var targetDragDropId = targetDragDropIdPool.Get(triggerEntityId).DragDropId;

                foreach (var objectEntityId in _filterObjects)
                {
                    if (objectIdPool.Get(objectEntityId).Id != targetObjectId)
                    {
                        continue;
                    }

                    foreach (var dragDropParameterEntityId in _filterDragDropParameters)
                    {
                        if (dragDropIdPool.Get(dragDropParameterEntityId).Id != targetDragDropId)
                        {
                            continue;
                        }
                        
                        // TODO: fix it!!!
                        var context = CurrentContext.Create(world);
                        context.Object.Push(objectEntityId);
                        context.GameVars.Update("target_place", targetPlaceId);
                        
                        var brick = dragDropActionPool.Get(dragDropParameterEntityId).Action;
                            
                        Debug.Log($"Action brick execute status {_serviceBricks.ExecuteBrick(brick, context, 1)}");
                            
                        // TODO: fix it!!!
                        _applyGameState.ApplySimulatedGameStates(context.GameStates);
                        CurrentContext.Destroy(world, context);
                        
                        break;
                    }

                    break;
                }
                
                world.DelEntity(triggerEntityId);
            }
        }
    }
}