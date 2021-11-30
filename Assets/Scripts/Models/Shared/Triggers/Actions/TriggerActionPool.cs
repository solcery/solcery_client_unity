using System.Collections.Generic;

namespace Solcery.Models.Shared.Triggers.Actions
{
    public sealed class TriggerActionPool
    {
        private Dictionary<TargetTriggerTypes, Dictionary<TriggerTypes, ITriggerAction>> _triggerPool;
    }
}