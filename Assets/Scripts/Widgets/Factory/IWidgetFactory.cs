using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Factory
{
    public interface IWidgetFactory
    {
        bool TryCreateWidget(JObject jsonData, out Widget widget);
        void Cleanup();
        void Destroy();
    }
}