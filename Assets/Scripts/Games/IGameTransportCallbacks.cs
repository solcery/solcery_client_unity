using Newtonsoft.Json.Linq;

namespace Solcery.Games
{
    public interface IGameTransportCallbacks
    {
        public void OnReceivingGameContent(JObject gameContentJson);
        public void OnReceivingGameState(JObject gameStateJson);
    }
}