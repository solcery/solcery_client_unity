using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates.Actions;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions
{
    public abstract class UpdateAction
    {
        public int StateId { get; }
        public ContextGameStateActionTypes ActionType { get; }

        protected UpdateAction(JObject data)
        {
            StateId = data.GetValue<int>("state_id");
            ActionType = data.GetEnum<ContextGameStateActionTypes>("action_type");
        }
    }
}