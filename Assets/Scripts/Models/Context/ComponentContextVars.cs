using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextVars : IEcsAutoReset<ComponentContextVars>
    {
        private Dictionary<string, object> _vars;

        public void Set(string key, object value)
        {
            if (!_vars.ContainsKey(key))
            {
                _vars.Add(key, value);
                return;
            }
            
            _vars[key] = value;
        }

        public bool TryGet(string key, out object value)
        {
            return _vars.TryGetValue(key, out value);
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_vars.TryGetValue(key, out var val) 
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
            if (!_vars.ContainsKey(key))
            {
                return;
            }

            _vars.Remove(key);
        }
        
        public void AutoReset(ref ComponentContextVars c)
        {
            c._vars ??= new Dictionary<string, object>();
            c._vars.Clear();
        }
    }
}