using Leopotam.EcsLite;

namespace Solcery.Models.Play.Actions.Animation.Move
{
    public struct ComponentAnimationMoveFromPlaceId : IEcsAutoReset<ComponentAnimationMoveFromPlaceId>
    {
        public int FromPlaceId;

        public void AutoReset(ref ComponentAnimationMoveFromPlaceId c)
        {
            c.FromPlaceId = -1;
        }
    }
}