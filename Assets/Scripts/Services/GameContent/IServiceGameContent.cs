using Newtonsoft.Json.Linq;
using Solcery.Services.GameContent.Items;

namespace Solcery.Services.GameContent
{
    public interface IServiceGameContent
    {
        IItemTypes ItemTypes { get; }
        JArray Places { get; }
        JArray DragDrop { get; }
        JArray Tooltips { get; }

        void UpdateGameContent(JObject data);
        void UpdateGameContentOverrides(JObject data);
        void Cleanup();
    }
}