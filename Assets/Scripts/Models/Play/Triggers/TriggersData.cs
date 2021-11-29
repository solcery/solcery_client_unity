using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Models.Play.Triggers
{
    public class TriggersData
    {
        private readonly Dictionary<TriggerTypes, JObject> _triggers;
        
        public static TriggersData Parse(JObject obj)
        {
            return new TriggersData(obj);
        }

        public bool TryGetValue(TriggerTypes type, out JObject trigger)
        {
            return _triggers.TryGetValue(type, out trigger);
        }

        private TriggersData(JObject obj)
        {
            _triggers = new Dictionary<TriggerTypes, JObject>();
            if (obj.TryGetValue("action", out JObject actionToken))
            {
                _triggers.Add(TriggerTypes.OnClick, actionToken);
            }
        }
    }
}