using Leopotam.EcsLite;

namespace Solcery.Models.Play.Actions.Animation.Visibility
{
    public struct ComponentAnimationVisibilityVisible : IEcsAutoReset<ComponentAnimationVisibilityVisible>
    {
        public bool Visible;

        public void AutoReset(ref ComponentAnimationVisibilityVisible c)
        {
            c.Visible = true;
        }
    }
}