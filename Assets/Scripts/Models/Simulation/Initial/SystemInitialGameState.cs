using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Simulation.Initial
{
    public interface ISystemInitialGameState : IEcsInitSystem { }

    public sealed class SystemInitialGameState : ISystemInitialGameState
    {
        public static ISystemInitialGameState Create(JObject initialGameState)
        {
            return new SystemInitialGameState(initialGameState);
        }
        
        private SystemInitialGameState(JObject initialGameState)
        {
            
        }
        
        public void Init(EcsSystems systems)
        {
            
        }
    }
}