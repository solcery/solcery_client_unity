using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.LocalSimulation;

namespace Solcery.Models.Simulation.Game.State
{
    public interface ISystemGameStateCreate : IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemGameStateCreate : ISystemGameStateCreate
    {
        private IServiceLocalSimulationApplyGameState _applyGameState;
        
        public static ISystemGameStateCreate Create(IServiceLocalSimulationApplyGameState applyGameState)
        {
            return new SystemGameStateCreate(applyGameState);
        }
        
        private SystemGameStateCreate(IServiceLocalSimulationApplyGameState applyGameState)
        {
            _applyGameState = applyGameState;
        }
        
        public void Run(EcsSystems systems)
        {
            var gameState = new JObject();
            
            _applyGameState.ApplySimulatedGameState(gameState);
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _applyGameState = null;
        }
    }
}