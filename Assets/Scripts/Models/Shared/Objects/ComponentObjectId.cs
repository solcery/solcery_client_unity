using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectId : IEcsAutoReset<ComponentObjectId>
    {
        public int Id;
        
        public void AutoReset(ref ComponentObjectId c)
        {
            c.Id = -1;
        }
    }
}