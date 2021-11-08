using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.Widget
{
    public class TriggersData
    {
        private readonly Dictionary<TriggerTypes, TriggerData> _triggers;
        
        public static TriggersData Parse(JToken arr)
        {
            return new TriggersData(arr);
        }

        public bool TryGetValue(TriggerTypes type, out TriggerData trigger)
        {
            return _triggers.TryGetValue(type, out trigger);
        }

        private TriggersData(JToken arr)
        {
            _triggers = new Dictionary<TriggerTypes, TriggerData>();
            foreach (var child in arr.Children())
            {
                var trigger = TriggerData.Parse(child.ToObject<JObject>());
                _triggers.Add(trigger.Type, trigger);
            }
        }
    }
}