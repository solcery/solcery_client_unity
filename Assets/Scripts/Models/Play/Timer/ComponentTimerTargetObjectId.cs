using Leopotam.EcsLite;

namespace Solcery.Models.Play.Timer
{
    public struct ComponentTimerTargetObjectId : IEcsAutoReset<ComponentTimerTargetObjectId>
    {
        public int TargetObjectId;

        public void AutoReset(ref ComponentTimerTargetObjectId c)
        {
            c.TargetObjectId = -1;
        }
    }
}