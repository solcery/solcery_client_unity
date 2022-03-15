using Newtonsoft.Json.Linq;

namespace Solcery.Games.Contexts.GameStates
{
    public sealed class ContextGameStateData : ContextGameState
    {
        public JObject GameStateData => _gameStateData;
        
        private readonly JObject _gameStateData;

        public static ContextGameState Create(JObject gameStateData)
        {
            return new ContextGameStateData(gameStateData);
        }

        private ContextGameStateData(JObject gameStateData)
        {
            _gameStateData = gameStateData;
        }
    }
}