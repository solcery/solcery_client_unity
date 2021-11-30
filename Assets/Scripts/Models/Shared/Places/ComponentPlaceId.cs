using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Places
{
    public struct ComponentPlaceId : IEcsAutoReset<ComponentPlaceId>
    {
        public int Id;

        public void AutoReset(ref ComponentPlaceId c)
        {
            c.Id = -1;
        }
    }
}