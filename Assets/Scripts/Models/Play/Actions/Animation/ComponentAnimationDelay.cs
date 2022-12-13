using Leopotam.EcsLite;

namespace Solcery.Models.Play.Actions.Animation
{
    public struct ComponentAnimationDelay : IEcsAutoReset<ComponentAnimationDelay>
    {
        public int DelayMSec;

        public void AutoReset(ref ComponentAnimationDelay c)
        {
            c.DelayMSec = 0;
        }
    }
}