using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Simulation.Initial
{
    public interface ISystemInitialGameContent : IEcsInitSystem { }

    public sealed class SystemInitialGameContent : ISystemInitialGameContent
    {
        private JObject _gameContent;
        
        public static ISystemInitialGameContent Create(JObject gameContent)
        {
            return new SystemInitialGameContent(gameContent);
        }
        
        private SystemInitialGameContent(JObject gameContent)
        {
            _gameContent = gameContent;
        }
        
        public void Init(EcsSystems systems)
        {
            
            
            _gameContent = null;
        }
    }
}