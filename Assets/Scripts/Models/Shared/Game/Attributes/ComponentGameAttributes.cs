using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Game.Attributes
{
    public struct ComponentGameAttributes : IEcsAutoReset<ComponentGameAttributes>
    {
        public Dictionary<string, IAttributeValue> Attributes;

        public void AutoReset(ref ComponentGameAttributes c)
        {
            c.Attributes ??= new Dictionary<string, IAttributeValue>();
            c.Attributes?.Clear();
        }
    }
}