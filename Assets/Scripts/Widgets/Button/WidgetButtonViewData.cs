using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Button
{
    public class WidgetButtonViewData : WidgetViewDataBase
    {
        public string Name { get; private set; }

        public override bool TryParse(JObject jsonData)
        {
            base.TryParse(jsonData);
            if (!jsonData.TryGetValue("name", out var name))
            {
                return false;
            }

            Name = name.Value<string>();
            return true;
        }        
    }
}