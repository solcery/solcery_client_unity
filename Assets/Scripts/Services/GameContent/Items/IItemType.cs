using Newtonsoft.Json.Linq;

namespace Solcery.Services.GameContent.Items
{
    public interface IItemType
    {
        int TplId { get; }
        void UpdateOverrides(JArray overrideKeys);
        bool TryGetValue(out JToken token, string key, int objectId = -1);
        void Cleanup();
    }
}