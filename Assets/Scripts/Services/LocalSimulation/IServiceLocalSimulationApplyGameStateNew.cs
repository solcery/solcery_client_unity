#if UNITY_EDITOR || LOCAL_SIMULATION
using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation
{
    public interface IServiceLocalSimulationApplyGameStateNew
    {
        void ApplySimulatedGameStates(JObject gameStates);
    }
}
#endif