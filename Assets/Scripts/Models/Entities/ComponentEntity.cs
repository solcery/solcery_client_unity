using Leopotam.EcsLite;

namespace Solcery.Models.Entities
{
    public struct ComponentEntity : IEcsAutoReset<ComponentEntity>
    {
        public int EntityId;
        
        public void AutoReset(ref ComponentEntity c)
        {
            c.EntityId = -1;
        }
    }
}