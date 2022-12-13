using Leopotam.EcsLite;
using Solcery.Widgets_new;

namespace Solcery.Models.Play.Actions.Animation.Rotate
{
    public struct ComponentAnimationRotateTargetFace : IEcsAutoReset<ComponentAnimationRotateTargetFace>
    {
        public PlaceWidgetCardFace TargetFace;

        public void AutoReset(ref ComponentAnimationRotateTargetFace c)
        {
            c.TargetFace = PlaceWidgetCardFace.Up;
        }
    }
}