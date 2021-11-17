using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Picture
{
    public class WidgetPictureViewData : WidgetViewDataBase
    {
        public string Picture { get; private set; }
        
        public override bool TryParse(JObject jsonData)
        {
            base.TryParse(jsonData);
            if (!jsonData.TryGetValue("picture", out var picture))
            {
                return false;
            }
            
            Picture = picture.Value<string>();
            return true;
        }
    }
}
