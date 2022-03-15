using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectIdHash : IEcsAutoReset<ComponentObjectIdHash>
    {
        public HashSet<int> ObjectIdHash;


        public void AutoReset(ref ComponentObjectIdHash c)
        {
            c.ObjectIdHash ??= new HashSet<int>();
            c.ObjectIdHash.Clear();
        }
    }
}