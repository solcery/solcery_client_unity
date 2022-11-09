using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation.GameStates
{
    public interface IServiceGameState
    {
        void PushGameState(JObject gameState);
        bool TryPopGameState(out JObject gameState);
    }
}