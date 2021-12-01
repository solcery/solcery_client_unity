using Leopotam.EcsLite;
using Solcery.Widgets;

namespace Solcery.Models.Play.Triggers
{
    public struct ComponentTriggerWidgetCollector : IEcsAutoReset<ComponentTriggerWidgetCollector>
    {
        public TriggerWidgetCollector TriggerCollector;

        public void AutoReset(ref ComponentTriggerWidgetCollector c)
        {
            c.TriggerCollector = null;
        }
 
    }
}