using Leopotam.EcsLite;

namespace Solcery.Models.Play.Places
{
    public struct ComponentPlaceIsVisible : IEcsAutoReset<ComponentPlaceIsVisible>
    {
        public bool IsVisible;
        
        public void AutoReset(ref ComponentPlaceIsVisible c)
        {
            c.IsVisible = false;
        }
    }
}