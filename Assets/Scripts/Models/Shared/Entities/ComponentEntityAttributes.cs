using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Entities
{
    public struct ComponentEntityAttributes : IEcsAutoReset<ComponentEntityAttributes>
    {
        public Dictionary<string, int> Attributes;

        public void AutoReset(ref ComponentEntityAttributes c)
        {
            c.Attributes ??= new Dictionary<string, int>();
            c.Attributes?.Clear();
        }
    }
}