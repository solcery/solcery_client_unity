using Leopotam.EcsLite;

namespace Solcery.Models.Play.Actions.Animation
{
    public struct ComponentAnimationObjectId : IEcsAutoReset<ComponentAnimationObjectId>
    {
        public int ObjectId;
        
        public void AutoReset(ref ComponentAnimationObjectId c)
        {
            c.ObjectId = -1;
        }
    }
}