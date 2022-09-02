using Leopotam.EcsLite;

namespace Solcery.Models.Play.Places
{
    public struct ComponentPlaceIsAvailable : IEcsAutoReset<ComponentPlaceIsAvailable>
    {
        public bool IsAvailable;
        
        public void AutoReset(ref ComponentPlaceIsAvailable c)
        {
            c.IsAvailable = false;
        }
    }
}