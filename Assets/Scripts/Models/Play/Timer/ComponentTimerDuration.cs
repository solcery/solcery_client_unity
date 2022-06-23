using Leopotam.EcsLite;

namespace Solcery.Models.Play.Timer
{
    public struct ComponentTimerDuration : IEcsAutoReset<ComponentTimerDuration>
    {
        public int DurationMsec;
        
        public void AutoReset(ref ComponentTimerDuration c)
        {
            c.DurationMsec = 0;
        }
    }
}