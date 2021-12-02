using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Context
{
    public struct ComponentContextVars : IEcsAutoReset<ComponentContextVars>
    {
        private Dictionary<string, int> _vars;

        public void Set(string key, int value)
        {
            if (!_vars.ContainsKey(key))
            {
                _vars.Add(key, value);
                return;
            }
            
            _vars[key] = value;
        }

        public bool TryGet(string key, out int value)
        {
            if (!_vars.ContainsKey(key))
            {
                Set(key, 0);
            }
            
            return _vars.TryGetValue(key, out value);
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
            c._vars ??= new Dictionary<string, int>();
            c._vars.Clear();
        }
    }
}