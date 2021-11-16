using Newtonsoft.Json.Linq;

namespace Solcery.Widgets
{
    public abstract class WidgetViewDataBase
    {
        public bool Enabled { get; private set; }

        public virtual bool TryParse(JObject jsonData)
        {
            if (jsonData.TryGetValue("enabled", out var enabled))
            {
                Enabled = enabled.Value<bool>();
            }

            return true;
        }
    }
}