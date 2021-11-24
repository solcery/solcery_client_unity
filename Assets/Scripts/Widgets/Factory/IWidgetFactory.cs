using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Factory
{
    public interface IWidgetFactory
    {
        bool TryCreateWidget(JObject jsonData, out IWidget widget);
        void Cleanup();
        void Destroy();
    }
}