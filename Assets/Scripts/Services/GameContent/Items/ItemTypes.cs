using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.GameContent.Items
{
    public sealed class ItemTypes : IItemTypes
    {
        IReadOnlyDictionary<int, IItemType> IItemTypes.Items => _items;
        
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

                var itemType = ItemType.Create(itemTypeObject);
                _items.Add(itemType.Id, itemType);
            }
        }

        void IItemTypes.UpdateOverridesItems(JArray itemOverrides)
        {
            foreach (var itemOverrideToken in itemOverrides)
            {
                if (itemOverrideToken is not JObject itemOverrideObject)
                {
                    continue;
                }

                var entityId = itemOverrideObject.GetValue<int>("id");
                var overridesArray = itemOverrideObject.GetValue<JArray>("overrides");

                foreach (var overrideToken in overridesArray)
                {
                    if (overrideToken is not JObject overrideObject)
                    {
                        continue;
                    }

                    var tplid = overrideObject.GetValue<int>("tpl_id");

                    if (_items.TryGetValue(tplid, out var itemType))
                    {
                        itemType.UpdateOverrides(entityId, overrideObject.GetValue<JObject>("data"));
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