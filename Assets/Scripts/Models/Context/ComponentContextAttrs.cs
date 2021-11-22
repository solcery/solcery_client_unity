using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextAttrs : IEcsAutoReset<ComponentContextAttrs>
    {
        public Dictionary<string, int> Attrs;
        
        public void AutoReset(ref ComponentContextAttrs c)
        {
            Attrs ??= new Dictionary<string, int>();
            Attrs.Clear();
        }
    }
}