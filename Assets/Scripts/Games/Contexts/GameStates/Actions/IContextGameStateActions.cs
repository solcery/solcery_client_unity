using Newtonsoft.Json.Linq;

namespace Solcery.Games.Contexts.GameStates.Actions
{
    public interface IContextGameStateActions
    {
        void Push();
        void AddAction(ContextGameStateAction action);
        void SetTargetStateId(int stateId);
        JArray ToJson();
        void Cleanup();
    }
}