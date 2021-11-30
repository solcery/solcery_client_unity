using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Play.Game.Attributes
{
    public struct ComponentGameAttributes : IEcsAutoReset<ComponentGameAttributes>
    {
        public Dictionary<string, int> Attributes;


        public void AutoReset(ref ComponentGameAttributes c)
        {
            c.Attributes ??= new Dictionary<string, int>();
            c.Attributes?.Clear();
        }
    }
}