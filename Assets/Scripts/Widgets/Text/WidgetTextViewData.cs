
using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Text
{
    public class WidgetTextViewData : WidgetViewDataBase
    {
        public string Description { get; private set; }
        
        public override bool TryParse(JObject jsonData)
        {
            base.TryParse(jsonData);
            if (!jsonData.TryGetValue("description", out var picture))
            {
                return false;
            }
            
            Description = picture.Value<string>();
            return true;
        }
    }
}
