using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.States;

namespace Solcery.Games.States.New
{
    public interface IUpdateStateQueue
    {
        UpdateState CurrentState { get; }
        void PushGameState(JObject gameState);
        void Update(int deltaTimeMsec);
    }
}