using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation.GameStates
{
    public interface IServiceGameState
    {
        bool IsEmpty { get; }

        void PushGameState(JObject gameState);
        bool TryPopGameState(out JObject gameState);
    }
}