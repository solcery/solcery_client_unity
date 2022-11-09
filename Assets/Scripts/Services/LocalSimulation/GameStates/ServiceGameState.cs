using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation.GameStates
{
    public sealed class ServiceGameState : IServiceGameState
    {
        private readonly Stack<JObject> _gameState;

        public static IServiceGameState Create()
        {
            return new ServiceGameState();
        }

        private ServiceGameState()
        {
            _gameState = new Stack<JObject>();
        }

        void IServiceGameState.PushGameState(JObject gameState)
        {
            _gameState.Clear();
            _gameState.Push(gameState);
        }

        bool IServiceGameState.TryPopGameState(out JObject gameState)
        {
            return _gameState.TryPop(out gameState);
        }
    }
}