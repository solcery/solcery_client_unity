using Newtonsoft.Json.Linq;

namespace Solcery.Games
{
    public interface IGameOnReceivingData
    {
        public void OnReceivingGameContent(JObject gameContentJson);
        public void OnReceivingGameState(JObject gameStateJson);
    }
}