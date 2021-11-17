using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Creator
{
    public interface IWidgetsCreator
    {
        public void Register(IWidgetCreator creator);
        public Widget CreateWidget(JObject data);
    }
}