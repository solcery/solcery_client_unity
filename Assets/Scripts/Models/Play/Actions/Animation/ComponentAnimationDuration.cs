using Leopotam.EcsLite;

namespace Solcery.Models.Play.Actions.Animation
{
    public struct ComponentAnimationDuration : IEcsAutoReset<ComponentAnimationDuration>
    {
        public int DurationMSec;
        
        public void AutoReset(ref ComponentAnimationDuration c)
        {
            c.DurationMSec = 0;
        }
    }
}