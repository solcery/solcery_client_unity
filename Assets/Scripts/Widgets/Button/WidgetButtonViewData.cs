using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Button
{
    public class WidgetButtonViewData
    {
        public string Name { get; private set; }

        public bool TryParse(JObject jsonData)
        {
            if (!jsonData.TryGetValue("name", out var name))
            {
                return false;
            }

            Name = name.Value<string>();
            return true;
        }        
    }
}