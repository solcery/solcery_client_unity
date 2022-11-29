using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Games.Contexts.GameStates.Actions.Action
{
    public sealed class ContextGameStateActionPushAction : ContextGameStateAction
    {
        private readonly int _actionType;
        private readonly IReadOnlyDictionary<string, int> _values;
        
        public static ContextGameStateAction Create(int actionType, IReadOnlyDictionary<string, int> values)
        {
            return new ContextGameStateActionPushAction(actionType, values);
        }

        private ContextGameStateActionPushAction(int actionType, IReadOnlyDictionary<string, int> values)
        {
            _actionType = actionType;
            _values = new Dictionary<string, int>(values);
        }

        public override JObject ToJson(int id, int stateId)
        {
            var result = new JObject
            {
                { "id", new JValue(id) },
                { "state_id", new JValue(stateId) },
                { "action_type", new JValue(_actionType) }
            };

            var value = new JObject();
            foreach (var val in _values)
            {
                value.Add(val.Key, new JValue(val.Value));
            }
            result.Add("value", value);

            return result;
        }
    }
}