using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation
{
    public interface IServiceLocalSimulationApplyGameState
    {
        void ApplySimulatedGameState(JObject gameState);
    }
}