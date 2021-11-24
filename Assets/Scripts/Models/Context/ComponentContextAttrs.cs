using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextAttrs : IEcsAutoReset<ComponentContextAttrs>
    {
        private Dictionary<string, object> _attrs;
        
        public void Set(string key, object value)
        {
            if (!_attrs.ContainsKey(key))
            {
                _attrs.Add(key, value);
                return;
            }
            
            _attrs[key] = value;
        }

        public bool TryGet(string key, out object value)
        {
            return _attrs.TryGetValue(key, out value);
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_attrs.TryGetValue(key, out var val) 
                && val is T valT)
            {
                value = valT;
                return true;
            }

            value = default;
            return false;
        }

        public void Remove(string key)
        {
            if (!_attrs.ContainsKey(key))
            {
                return;
            }

            _attrs.Remove(key);
        }
        
        public void AutoReset(ref ComponentContextAttrs c)
        {
            _attrs ??= new Dictionary<string, object>();
            _attrs.Clear();
        }
    }
}