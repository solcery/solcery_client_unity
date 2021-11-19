using Solcery.Widgets;

namespace Solcery.Models.Places
{
    public struct ComponentPlaceSubWidget
    {
        public Widget Widget;

        public void AutoReset(ref ComponentPlaceWidget c)
        {
            c.Widget = null;
        }
    }
}