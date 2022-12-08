using Newtonsoft.Json.Linq;

namespace Solcery.Games.Contexts.GameStates.Actions.Action
{
    public sealed class ContextGameStateActionPushActionJToken : ContextGameStateAction
    {
        private readonly int _actionType;
        private readonly JToken _values;
        
        public static ContextGameStateAction Create(int actionType, JToken values)
        {
            return new ContextGameStateActionPushActionJToken(actionType, values);
        }

        private ContextGameStateActionPushActionJToken(int actionType, JToken values)
        {
            _actionType = actionType;
            _values = values;
        }

        public override JObject ToJson(int id, int stateId)
        {
            var result = new JObject
            {
                { "id", new JValue(id) },
                { "state_id", new JValue(stateId) },
                { "action_type", new JValue(_actionType) },
                { "value", _values}
            };

            return result;
        }
    }
}