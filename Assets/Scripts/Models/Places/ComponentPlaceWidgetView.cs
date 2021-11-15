using Solcery.Widgets;

namespace Solcery.Models.Places
{
    public struct ComponentPlaceWidgetView
    {
        public WidgetViewBase View;

        public void AutoReset(ref ComponentPlaceWidgetView c)
        {
            c.View = null;
        }        
    }
}