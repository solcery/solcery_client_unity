using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextObject : IEcsAutoReset<ComponentContextObject>
    {
        public object Object;
        
        public void AutoReset(ref ComponentContextObject c)
        {
            c.Object = null;
        }
    }
}