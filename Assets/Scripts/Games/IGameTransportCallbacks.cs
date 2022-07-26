using Newtonsoft.Json.Linq;

namespace Solcery.Games
{
    public interface IGameTransportCallbacks
    {
        void OnReceivingGameContent(JObject gameContentJson);
        void OnReceivingGameContentOverrides(JObject gameContentOverridesJson);
        void OnReceivingGameState(JObject gameStateJson);
    }
}