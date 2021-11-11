using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Game
{
    public struct ComponentGameAttributes : IEcsAutoReset<ComponentGameAttributes>
    {
        public Dictionary<string, int> Attributes;


        public void AutoReset(ref ComponentGameAttributes c)
        {
            c.Attributes?.Clear();
        }
    }
}