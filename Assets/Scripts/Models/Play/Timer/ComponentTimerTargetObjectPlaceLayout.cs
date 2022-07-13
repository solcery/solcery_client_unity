using Leopotam.EcsLite;
using Solcery.Widgets_new;

namespace Solcery.Models.Play.Timer
{
    public struct ComponentTimerTargetObjectPlaceLayout : IEcsAutoReset<ComponentTimerTargetObjectPlaceLayout>
    {
        public PlaceWidgetLayout Layout;

        public void AutoReset(ref ComponentTimerTargetObjectPlaceLayout c)
        {
            c.Layout = null;
        }
    }
}