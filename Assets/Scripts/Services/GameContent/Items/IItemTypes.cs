using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.GameContent.Items
{
    public interface IItemTypes
    {
        List<string> PictureUriList { get; }
        IReadOnlyDictionary<int, IItemType> Items { get; }
        void UpdateOverridesItems(JObject itemOverrides);
        bool TryGetItemType(out IItemType itemType, int tplid);
        void Cleanup();
    }
}