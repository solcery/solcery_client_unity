using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates.Actions;

namespace Solcery.Games.States.New.Actions.Animation
{
    public sealed class UpdateActionAnimationEmpty : UpdateAction
    {
        public static UpdateAction Create()
        {
            return new UpdateActionAnimationEmpty(new JObject
            {
                {"id", new JValue(-1)},
                {"state_id", new JValue(-1)},
                {"action_type", new JValue((int)ContextGameStateActionTypes.None)}
            });
        }
        
        private UpdateActionAnimationEmpty(JObject data) : base(data) { }
    }
}