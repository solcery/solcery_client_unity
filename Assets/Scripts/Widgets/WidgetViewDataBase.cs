using Newtonsoft.Json.Linq;

namespace Solcery.Widgets
{
    public abstract class WidgetViewDataBase
    {
        public virtual bool TryParse(JObject jsonData)
        {
            return true;
        }
    }
}