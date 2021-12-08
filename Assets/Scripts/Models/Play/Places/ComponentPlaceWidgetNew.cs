using Leopotam.EcsLite;
using Solcery.Widgets_new;

namespace Solcery.Models.Play.Places
{
    public struct ComponentPlaceWidgetNew : IEcsAutoReset<ComponentPlaceWidgetNew>
    {
        public PlaceWidget Widget;
        
        public void AutoReset(ref ComponentPlaceWidgetNew c)
        {
            c.Widget?.Destroy();
            c.Widget = null;
        }
    }
}