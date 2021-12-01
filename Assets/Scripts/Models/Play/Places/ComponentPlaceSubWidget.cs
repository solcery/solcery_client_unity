using Leopotam.EcsLite;
using Solcery.Widgets;

namespace Solcery.Models.Play.Places
{
    public struct ComponentPlaceSubWidget : IEcsAutoReset<ComponentPlaceSubWidget>
    {
        public Widget Widget;

        public void AutoReset(ref ComponentPlaceSubWidget c)
        {
            c.Widget = null;
        }
    }
}