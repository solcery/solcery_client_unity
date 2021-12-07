using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectTypes : IEcsAutoReset<ComponentObjectTypes>
    {
        public Dictionary<int, JObject> Types;

        public void AutoReset(ref ComponentObjectTypes c)
        {
            c.Types ??= new Dictionary<int, JObject>();
            c.Types?.Clear();
        }
    }
}