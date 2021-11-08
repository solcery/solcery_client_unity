using Newtonsoft.Json.Linq;

namespace Solcery.Services.Widget
{
    public class TriggerData
    {
        public TriggerTypes Type;
        // todo use object if will need
        public string Action;
    
        public static TriggerData Parse(JObject obj)
        {
            return new TriggerData(obj);
        }

        private TriggerData(JObject obj)
        {
            Type = obj["event"]!.ToObject<TriggerTypes>();
            if (obj.TryGetValue("action", out var action))
            {
                Action = action["type"]!.Value<string>();
            }
        }
    }
}