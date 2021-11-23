using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextVars : IEcsAutoReset<ComponentContextVars>
    {
        public Dictionary<string, object> Vars;

        public void SetVar(string key, object value)
        {
            if (Vars.ContainsKey(key))
            {
                Vars[key] = value;
            }
            else
            {
                Vars.Add(key, value);
            }
        }

        public bool TryGetVar(string key, out object value)
        {
            return Vars.TryGetValue(key, out value);
        }
        
        public void AutoReset(ref ComponentContextVars c)
        {
            Vars ??= new Dictionary<string, object>();
            Vars.Clear();
        }
    }
}