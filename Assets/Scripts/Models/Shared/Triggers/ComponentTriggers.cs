using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Shared.Triggers.Actions;

namespace Solcery.Models.Shared.Triggers
{
    public struct ComponentTriggers : IEcsAutoReset<ComponentTriggers>
    {
        private Dictionary<TriggerTypes, ITriggerAction> _triggerActions;

        public void AddTriggerForType(TriggerTypes triggerType, ITriggerAction trigger)
        {
            if (!_triggerActions.ContainsKey(triggerType))
            {
                _triggerActions.Add(triggerType, trigger);
                return;
            }

            _triggerActions[triggerType] = trigger;
        }

        public bool TryGetTriggerForType(TriggerTypes triggerType, out ITriggerAction trigger)
        {
            return _triggerActions.TryGetValue(triggerType, out trigger);
        }

        public void AutoReset(ref ComponentTriggers c)
        {
            _triggerActions ??= new Dictionary<TriggerTypes, ITriggerAction>((int)TriggerTypes.Count);
            _triggerActions?.Clear();
        }
    }
}