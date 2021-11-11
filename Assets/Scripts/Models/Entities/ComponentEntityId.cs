using Leopotam.EcsLite;

namespace Solcery.Models.Entities
{
    public struct ComponentEntityId : IEcsAutoReset<ComponentEntityId>
    {
        public int Id;
        
        public void AutoReset(ref ComponentEntityId c)
        {
            c.Id = -1;
        }
    }
}