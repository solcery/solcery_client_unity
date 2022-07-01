using Leopotam.EcsLite;

namespace Solcery.Models.Play.Timer
{
    public struct ComponentTimerFinishTime : IEcsAutoReset<ComponentTimerFinishTime>
    {
        public float FinishTime;
        
        public void AutoReset(ref ComponentTimerFinishTime c)
        {
            c.FinishTime = 0f;
        }
    }
}