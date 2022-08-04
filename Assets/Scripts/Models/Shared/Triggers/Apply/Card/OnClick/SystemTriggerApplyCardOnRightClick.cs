using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Games.Contexts;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types.OnClick;
using Solcery.Services.LocalSimulation;
using UnityEngine;

namespace Solcery.Models.Shared.Triggers.Apply.Card.OnClick
{
    public interface ISystemTriggerApplyCardOnRightClick : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }
    
    public class SystemTriggerApplyCardOnRightClick : ISystemTriggerApplyCardOnRightClick
    {
        private readonly IServiceLocalSimulationApplyGameStateNew _applyGameState;
        private EcsFilter _filterTriggers;
        private EcsFilter _filterEntities;
        
        public static ISystemTriggerApplyCardOnRightClick Create(IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            return new SystemTriggerApplyCardOnRightClick(applyGameState);
        }
        
        private SystemTriggerApplyCardOnRightClick(IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            _applyGameState = applyGameState;
        }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            _filterTriggers = world.Filter<ComponentTriggerTag>()
                .Inc<ComponentTriggerTargetObjectId>()
                .Inc<ComponentTriggerEntityCardTag>()
                .Inc<ComponentTriggerOnRightClickTag>()
                .End();

            _filterEntities = world.Filter<ComponentObjectTag>().Inc<ComponentObjectId>().Inc<ComponentObjectType>()
                .End();
        }
        
        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            _filterTriggers = null;
            _filterEntities = null;
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            var world = systems.GetWorld();
            var triggerTargetEntityIdPool = world.GetPool<ComponentTriggerTargetObjectId>();
            var entityIdPool = world.GetPool<ComponentObjectId>();
            var entityTypePool = world.GetPool<ComponentObjectType>();

            foreach (var triggerEntityId in _filterTriggers)
            {
                var targetObjectId = triggerTargetEntityIdPool.Get(triggerEntityId).TargetObjectId;

                foreach (var entityId in _filterEntities)
                {
                    if (entityIdPool.Get(entityId).Id != targetObjectId)
                    {
                        continue;
                    }

                    var tplid = entityTypePool.Get(entityId).TplId;

                    if (!game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid)
                        || !itemType.TryGetValue(out var valueBrickToken, "action_on_right_click", targetObjectId)
                        || valueBrickToken is not JObject brick)
                    {
                        Debug.Log("Can't find action_on_right_click!");
                        continue;
                    }

                    
                    var context = CurrentContext.Create(game, world);
                    context.Object.Push(entityId);
                    Debug.Log($"Action brick execute status {game.ServiceBricks.ExecuteBrick(brick, context, 1)}");
                    _applyGameState.ApplySimulatedGameStates(context.GameStates);
                    CurrentContext.Destroy(world, context);

                    break;
                }

                world.DelEntity(triggerEntityId);
            }
        }
    }
}