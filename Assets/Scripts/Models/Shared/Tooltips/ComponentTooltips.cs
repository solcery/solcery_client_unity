using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Shared.Tooltips
{
    public struct ComponentTooltips : IEcsAutoReset<ComponentTooltips>
    {
        public Dictionary<int, JObject> Tooltips;

        public void AutoReset(ref ComponentTooltips c)
        {
            c.Tooltips ??= new Dictionary<int, JObject>();
            c.Tooltips?.Clear();
        }
        
    }
}