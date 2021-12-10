using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectAttributes : IEcsAutoReset<ComponentObjectAttributes>
    {
        public Dictionary<string, IAttributeValue> Attributes;

        public void AutoReset(ref ComponentObjectAttributes c)
        {
            c.Attributes ??= new Dictionary<string, IAttributeValue>();
            c.Attributes?.Clear();
        }
    }
}