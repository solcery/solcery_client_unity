using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextVars : IEcsAutoReset<ComponentContextVars>
    {
        public Dictionary<string, object> Vars;
        
        public void AutoReset(ref ComponentContextVars c)
        {
            Vars ??= new Dictionary<string, object>();
            Vars.Clear();
        }
    }
}