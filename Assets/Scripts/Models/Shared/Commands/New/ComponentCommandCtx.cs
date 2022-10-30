using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Commands.New
{
    public struct ComponentCommandCtx : IEcsAutoReset<ComponentCommandCtx>
    {
        public Dictionary<string, int> Ctx;
        
        public void AutoReset(ref ComponentCommandCtx c)
        {
            c.Ctx ??= new Dictionary<string, int>();
            c.Ctx.Clear();
        }
    }
}