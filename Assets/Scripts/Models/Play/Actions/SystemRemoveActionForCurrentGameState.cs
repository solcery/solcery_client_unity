using Leopotam.EcsLite;
using Solcery.Games;

namespace Solcery.Models.Play.Actions
{
    public interface ISystemRemoveActionForCurrentGameState : IEcsRunSystem { }

    public sealed class SystemRemoveActionForCurrentGameState : ISystemRemoveActionForCurrentGameState
    {
        public static ISystemRemoveActionForCurrentGameState Create()
        {
            return new SystemRemoveActionForCurrentGameState();
        }
        
        private SystemRemoveActionForCurrentGameState() { }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            var updateState = game.UpdateStateQueue.CurrentState;
            var stateId = updateState?.StateId ?? -1;
            game.UpdateStateQueue.RemoveAllActionForStateId(stateId);
        }
    }
}