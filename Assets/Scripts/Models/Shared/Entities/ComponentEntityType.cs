using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Entities
{
    public struct ComponentEntityType : IEcsAutoReset<ComponentEntityType>
    {
        public int Type;

        public void AutoReset(ref ComponentEntityType c)
        {
            c.Type = -1;
        }
    }
}