using Leopotam.EcsLite;
using Solcery.Widgets;

namespace Solcery.Models.Places
{
    public struct ComponentPlaceWidget : IEcsAutoReset<ComponentPlaceWidget>
    {
        public Widget Widget;

        public void AutoReset(ref ComponentPlaceWidget c)
        {
            Widget = null;
        }
    }
}