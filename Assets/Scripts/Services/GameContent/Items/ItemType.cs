using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.GameContent.Items
{
    public sealed class ItemType : IItemType
    {
        int IItemType.TplId => _tplId;
        
        private readonly int _tplId;
        private readonly Dictionary<string, JToken> _defaultData;
        private readonly HashSet<string> _overrideKeys;

        private IItemTypesPrivate _itemTypesPrivate;

        public static IItemType Create(IItemTypesPrivate itemTypesPrivate, JObject data)
        {
            return new ItemType(itemTypesPrivate, data);
        }

        private ItemType(IItemTypesPrivate itemTypesPrivate, JObject data)
        {
            _itemTypesPrivate = itemTypesPrivate;
            
            _tplId = data.GetValue<int>("id");
            _defaultData = new Dictionary<string, JToken>();
            _overrideKeys = new HashSet<string>();

            foreach (var property in data)
            {
                if (property.Key == "id" 
                    || property.Value == null)
                {
                    continue;
                }
                
                _defaultData.Add(property.Key, property.Value);
            }
        }
        
        void IItemType.UpdateOverrides(JArray overrideKeys)
        {
            foreach (var overrideKeyToken in overrideKeys)
            {
                var overrideKey = overrideKeyToken.GetValue<string>();
                if (!_overrideKeys.Contains(overrideKey))
                {
                    _overrideKeys.Add(overrideKey);
                }
            }
        }

        bool IItemType.TryGetValue(out JToken token, string key, int objectId)
        {
            if (_overrideKeys.Contains(key)
                && _itemTypesPrivate.TryGetOverride(out token, key, objectId))
            {
                return true;
            }

            return _defaultData.TryGetValue(key, out token);
        }

        void IItemType.Cleanup()
        {
            _itemTypesPrivate = null;
            _defaultData.Clear();
            _overrideKeys.Clear();
        }
    }
}