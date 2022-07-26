using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.GameContent.Items
{
    public interface IItemTypes
    {
        IReadOnlyDictionary<int, IItemType> Items { get; }
        void UpdateOverridesItems(JArray itemOverrides);
        bool TryGetItemType(out IItemType itemType, int tplid);
        void Cleanup();
    }
}