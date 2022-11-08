using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Games.Contexts;
using Solcery.Services.LocalSimulation;
using UnityEngine;

namespace Solcery.Models.Shared.Commands.New
{
    public interface ISystemExecuteCommandNew : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemExecuteCommandNew : ISystemExecuteCommandNew
    {
        private readonly IServiceLocalSimulationApplyGameStateNew _applyGameState;
        private EcsFilter _filterCommandsForExecute;

        public static ISystemExecuteCommandNew Create(IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            return new SystemExecuteCommandNew(applyGameState);
        }
        
        private SystemExecuteCommandNew(IServiceLocalSimulationApplyGameStateNew applyGameState)
        {
            _applyGameState = applyGameState;
        }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            _filterCommandsForExecute = world.Filter<ComponentCommandTag>()
                .Inc<ComponentCommandId>()
                .Inc<ComponentCommandCtx>()
                .Exc<ComponentCommandConsumeTag>()
                .End();
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            var world = systems.GetWorld();
            var poolCommandId = world.GetPool<ComponentCommandId>();
            var poolCommandCtx = world.GetPool<ComponentCommandCtx>();
            var poolCommandConsumeTag = world.GetPool<ComponentCommandConsumeTag>();
            
            foreach (var entityId in _filterCommandsForExecute)
            {
                var commandId = poolCommandId.Get(entityId).Id;
                if (game.ServiceGameContent.TryGetCommand(commandId, out var brick))
                {
                    var context = CurrentContext.Create(game, world);
                    var localScopes = context.LocalScopes.New();
                    var commandCtx = poolCommandCtx.Get(entityId).Ctx;
                    foreach (var ctx in commandCtx)
                    {
                        localScopes.Vars.Update(ctx.Key, ctx.Value);
                    }
                    Debug.Log($"Action brick execute status {game.ServiceBricks.ExecuteBrick(brick, context, 1)}");
                    _applyGameState.ApplySimulatedGameStates(context.GameStates);
                    context.LocalScopes.Pop();
                    CurrentContext.Destroy(world, context);
                }

                poolCommandConsumeTag.Add(entityId);
            }
        }

        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            _filterCommandsForExecute = null;
        }
    }
}