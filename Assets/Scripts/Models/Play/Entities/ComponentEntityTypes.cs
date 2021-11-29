using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Play.Entities
{
    public struct ComponentEntityTypes : IEcsAutoReset<ComponentEntityTypes>
    {
        public Dictionary<int, JObject> Types;

        public void AutoReset(ref ComponentEntityTypes c)
        {
            c.Types?.Clear();
        }
    }
}