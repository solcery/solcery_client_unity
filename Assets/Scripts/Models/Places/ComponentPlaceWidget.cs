using Leopotam.EcsLite;
using Solcery.Widgets;

namespace Solcery.Models.Places
{
    public struct ComponentPlaceWidget : IEcsAutoReset<ComponentPlaceWidget>
    {
        public IWidget Widget;

        public void AutoReset(ref ComponentPlaceWidget c)
        {
            c.Widget = null;
        }
    }
}