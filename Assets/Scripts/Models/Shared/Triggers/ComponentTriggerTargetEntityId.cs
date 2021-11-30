using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Triggers
{
    public struct ComponentTriggerTargetEntityId : IEcsAutoReset<ComponentTriggerTargetEntityId>
    {
        public int TargetEntityId;

        public void AutoReset(ref ComponentTriggerTargetEntityId c)
        {
            c.TargetEntityId = -1;
        }
    }
}