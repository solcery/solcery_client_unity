using Leopotam.EcsLite;
using Solcery.Widgets_new;

namespace Solcery.Models.Play.Actions.Animation.Move
{
    public struct ComponentAnimationMoveFace : IEcsAutoReset<ComponentAnimationMoveFace>
    {
        public PlaceWidgetCardFace Face;

        public void AutoReset(ref ComponentAnimationMoveFace c)
        {
            c.Face = PlaceWidgetCardFace.Up;
        }
    }
}