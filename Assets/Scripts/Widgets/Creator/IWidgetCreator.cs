using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets.Creator
{
    public interface IWidgetCreator
    {
        public bool TryCreate(JObject jsonData, out Widget widget);
    }
}