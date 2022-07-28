using Newtonsoft.Json.Linq;

namespace Solcery.Services.GameContent.Items
{
    public interface IItemTypesPrivate
    {
        bool TryGetOverride(out JToken value, string key, int objectId);
    }
}