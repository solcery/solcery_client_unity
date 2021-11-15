using Solcery.Widgets;

namespace Solcery.Models.Places
{
    public struct ComponentEntityView
    {
        public WidgetViewBase View;

        public void AutoReset(ref ComponentEntityView c)
        {
            c.View = null;
        }        
    }
}