using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Triggers
{
    public class TriggersData
    {
        private readonly Dictionary<TriggerTypes, JToken> _triggers;
        
        public static TriggersData Parse(JObject obj)
        {
            return new TriggersData(obj);
        }

        public bool TryGetValue(TriggerTypes type, out JToken trigger)
        {
            return _triggers.TryGetValue(type, out trigger);
        }

        private TriggersData(JObject obj)
        {
            _triggers = new Dictionary<TriggerTypes, JToken>();
            if (obj.TryGetValue("action", out var actionToken))
            {
                _triggers.Add(TriggerTypes.OnClick, actionToken);
            }
        }
    }
}