using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.GameContent.Items
{
    public sealed class ItemTypes : IItemTypes, IItemTypesPrivate
    {
        IReadOnlyDictionary<int, IItemType> IItemTypes.Items => _items;

        private readonly Dictionary<int, Dictionary<string, JToken>> _overrides;
        private readonly Dictionary<int, IItemType> _items;

        public static IItemTypes Create(JArray itemTypes)
        {
            return new ItemTypes(itemTypes);
        }

        private ItemTypes(JArray itemTypes)
        {
            _items = new Dictionary<int, IItemType>(itemTypes.Count);

            foreach (var itemTypeToken in itemTypes)
            {
                if (itemTypeToken is not JObject itemTypeObject)
                {
                    continue;
                }

                var itemType = ItemType.Create(this, itemTypeObject);
                _items.Add(itemType.TplId, itemType);
            }

            _overrides = new Dictionary<int, Dictionary<string, JToken>>();
        }

        void IItemTypes.UpdateOverridesItems(JObject itemOverrides)
        {
            if (itemOverrides.TryGetValue("nfts", out JArray nftsArray))
            {
                foreach (var nftToken in nftsArray)
                {
                    if (nftToken is JObject nftObject
                        && nftObject.TryGetValue("id", out int objectId)
                        && nftObject.TryGetValue("data", out JObject dataObject))
                    {
                        _overrides.Add(objectId, new Dictionary<string, JToken>());
                        foreach (var data in dataObject)
                        {
                            _overrides[objectId].Add(data.Key, data.Value);
                        }
                    }
                }
            }

            if (itemOverrides.TryGetValue("card_types", out JArray ctArray))
            {
                foreach (var ctToken in ctArray)
                {
                    if (ctToken is JObject ctObject
                        && ctObject.TryGetValue("id", out int tplId)
                        && ctObject.TryGetValue("override_fields", out JArray overrideKeys)
                        && _items.TryGetValue(tplId, out var itemType))
                    {
                        itemType.UpdateOverrides(overrideKeys);
                    }
                }
            }
        }

        bool IItemTypes.TryGetItemType(out IItemType itemType, int tplid)
        {
            itemType = null;
            if (_items.TryGetValue(tplid, out itemType))
            {
                return true;
            }
            
            return false;
        }

        bool IItemTypesPrivate.TryGetOverride(out JToken value, string key, int objectId)
        {
            value = null;
            if (_overrides.TryGetValue(objectId, out var dict)
                && dict.TryGetValue(key, out value))
            {
                return true;
            }

            return false;
        }

        void IItemTypes.Cleanup()
        {
            foreach (var itemType in _items)
            {
                itemType.Value.Cleanup();
            }
            _items.Clear();
        }
    }
}