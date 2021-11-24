using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextArgs : IEcsAutoReset<ComponentContextAttrs>
    {
        private Dictionary<string, object> _args;
        
        public void Set(string key, object value)
        {
            if (!_args.ContainsKey(key))
            {
                _args.Add(key, value);
                return;
            }
            
            _args[key] = value;
        }

        public bool TryGet(string key, out object value)
        {
            return _args.TryGetValue(key, out value);
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_args.TryGetValue(key, out var val) 
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
            if (!_args.ContainsKey(key))
            {
                return;
            }

            _args.Remove(key);
        }
        
        public void AutoReset(ref ComponentContextAttrs c)
        {
            _args ??= new Dictionary<string, object>();
            _args.Clear();
        }
    }
}