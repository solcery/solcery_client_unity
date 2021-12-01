using System.Collections.Generic;
using Solcery.Models.Play.Attributes.Interactable;
using Solcery.Models.Shared.Triggers.Types;
using Solcery.Widgets;

namespace Solcery.Models.Play.Triggers
{
    public class TriggerWidgetData
    {
        public TriggerTypes Type;
        public int EntityId;
    }

    public class TriggerWidgetCollector : ITriggerWidgetCollector
    {
        private readonly Stack<TriggerWidgetData> _triggers;

        public TriggerWidgetCollector()
        {
            _triggers = new Stack<TriggerWidgetData>();
        }

        public void Register(int entityId, IWidget widget)
        {
            if (widget.View is IInteractable interactable)
            {
                interactable.OnClick = () =>
                {
                    _triggers.Push(new TriggerWidgetData
                    {
                        EntityId = entityId,
                        Type = TriggerTypes.OnClick
                    });
                };
            }
        }

        public bool TryGet(out TriggerWidgetData data)
        {
            if (HasTriggers())
            {
                data = _triggers.Pop();
                return true;
            }

            data = null;
            return false;
        }

        public bool HasTriggers()
        {
            return _triggers.Count > 0;
        }

        public void Cleanup()
        {
            _triggers.Clear();
        }
    }
}