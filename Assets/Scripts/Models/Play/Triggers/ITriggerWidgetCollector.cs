using Solcery.Widgets;

namespace Solcery.Models.Play.Triggers
{
    public interface ITriggerWidgetCollector
    {
        public bool TryGet(out TriggerWidgetData data);
        public void Register(int entityId, IWidget widget);
        public bool HasTriggers();
    }
}