using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectAttributes : IEcsAutoReset<ComponentObjectAttributes>
    {
        public Dictionary<string, int> Attributes;

        public void AutoReset(ref ComponentObjectAttributes c)
        {
            c.Attributes ??= new Dictionary<string, int>();
            c.Attributes?.Clear();
        }
    }
}