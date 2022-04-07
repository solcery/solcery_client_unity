using Newtonsoft.Json.Linq;

namespace Solcery.Games.States
{
    public sealed class GameState : State
    {
        public readonly JObject GameStateObject;
        
        public static GameState Create(JObject gameState)
        {
            return new GameState(gameState);
        }
        
        private GameState(JObject gameState)
        {
            GameStateObject = gameState;
        }
    }
}