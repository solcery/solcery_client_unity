using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.Widget.Data
{
    public class TriggerData
    {
        public readonly TriggerTypes Type;
        public readonly List<JObject> Actions;
    
        public static TriggerData Parse(JObject obj)
        {
            return new TriggerData(obj);
        }

        private TriggerData(JObject obj)
        {
            Type = obj["event"]!.ToObject<TriggerTypes>();

            Actions = new List<JObject>();
            if (obj.TryGetValue("actions", out JArray actionsArray))
            {
                foreach (var action in actionsArray)
                {
                    Actions.Add((JObject)action);
                }
            }
        }
    }
}