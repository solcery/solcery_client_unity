using Newtonsoft.Json.Linq;

namespace Solcery.Services.GameContent.Items
{
    public interface IItemType
    {
        int Id { get; }
        void UpdateOverrides(int entityId, JObject data);
        bool TryGetValue(out JToken token, string key, int entityId = -1);
        void Cleanup();
    }
}