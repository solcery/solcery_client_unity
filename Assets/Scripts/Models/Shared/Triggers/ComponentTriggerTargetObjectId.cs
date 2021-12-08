using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Triggers
{
    public struct ComponentTriggerTargetObjectId : IEcsAutoReset<ComponentTriggerTargetObjectId>
    {
        public int TargetObjectId;

        public void AutoReset(ref ComponentTriggerTargetObjectId c)
        {
            c.TargetObjectId = -1;
        }
    }
}