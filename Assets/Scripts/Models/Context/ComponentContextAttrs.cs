using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextAttrs : IEcsAutoReset<ComponentContextAttrs>
    {
        public Dictionary<string, object> Attrs;
        
        public void AutoReset(ref ComponentContextAttrs c)
        {
            Attrs ??= new Dictionary<string, object>();
            Attrs.Clear();
        }
    }
}