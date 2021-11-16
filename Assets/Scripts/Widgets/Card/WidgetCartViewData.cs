using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Card
{
    public class WidgetCartViewData
    {
        public string Picture { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public bool TryParse(JObject jsonData)
        {
            if (!jsonData.TryGetValue("picture", out var picture) ||
                !jsonData.TryGetValue("name", out var name) || 
                !jsonData.TryGetValue("description", out var description) )
            {
                return false;
            }

            Picture = picture.Value<string>();
            Name = name.Value<string>();
            Description = description.Value<string>();
            return true;
        }
    }}