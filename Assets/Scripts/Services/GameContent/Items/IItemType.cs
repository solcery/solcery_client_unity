using Newtonsoft.Json.Linq;

namespace Solcery.Services.GameContent.Items
{
    public interface IItemType
    {
        int Id { get; }
        void UpdateOverrides(int entityId, JObject data);
        bool TryGetValue(out JToken value, string key, int entityId);
        void Cleanup();
    }
}