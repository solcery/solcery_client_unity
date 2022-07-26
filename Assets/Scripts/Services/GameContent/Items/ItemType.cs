using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.GameContent.Items
{
    public sealed class ItemType : IItemType
    {
        int IItemType.Id => _id;
        
        private readonly int _id;
        private readonly Dictionary<string, JToken> _defaultData;
        private readonly Dictionary<int, Dictionary<string, JToken>> _data;

        public static IItemType Create(JObject data)
        {
            return new ItemType(data);
        }

        private ItemType(JObject data)
        {
            _id = data.GetValue<int>("id");
            _data = new Dictionary<int, Dictionary<string, JToken>>();
            _defaultData = new Dictionary<string, JToken>();

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
        
        void IItemType.UpdateOverrides(int entityId, JObject data)
        {
            if (!_data.ContainsKey(entityId))
            {
                _data.Add(entityId, new Dictionary<string, JToken>());
            }

            var dic = _data[entityId];
            foreach (var property in data)
            {
                if (!dic.ContainsKey(property.Key))
                {
                    dic.Add(property.Key, property.Value);
                    continue;
                }

                dic[property.Key] = property.Value;
            }
        }

        bool IItemType.TryGetValue(out JToken value, string key, int entityId)
        {
            value = null;

            if (!_data.ContainsKey(entityId) && _defaultData.TryGetValue(key, out value))
            {
                return true;
            }

            if (_data.TryGetValue(entityId, out var subData)
                && subData.TryGetValue(key, out value))
            {
                return true;
            }
            
            return false;
        }

        void IItemType.Cleanup()
        {
            _defaultData.Clear();
            _data.Clear();
        }
    }
}