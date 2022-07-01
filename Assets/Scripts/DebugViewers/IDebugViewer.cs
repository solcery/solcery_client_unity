using Newtonsoft.Json.Linq;

namespace Solcery.DebugViewers
{
    public interface IDebugViewer
    {
        void Show();
        void Hide();
        void AddGameStatePackage(JObject gameStateJson);
    }
}