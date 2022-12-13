using Leopotam.EcsLite;
using Solcery.Games.States.New.Actions;
using Solcery.Games.States.New.Actions.Animation.Rotate;
using Solcery.Models.Play.Actions.Factory;
using UnityEngine;

namespace Solcery.Models.Play.Actions.Animation.Rotate
{
    public static class CreatorAnimationRotate
    {
        public static bool Create(IActionObjectFactory factory, EcsWorld world, UpdateAction action)
        {
            if (action is UpdateActionAnimationRotate actionAnimationRotate)
            {
                var dt = (int)(Time.deltaTime * 1000);
                var entityId = world.NewEntity();
                world.GetPool<ComponentAnimationRotateTag>().Add(entityId);
                world.GetPool<ComponentAnimationObjectId>().Add(entityId).ObjectId = actionAnimationRotate.ObjectId;
                world.GetPool<ComponentAnimationDuration>().Add(entityId).DurationMSec = actionAnimationRotate.DurationMsec;
                world.GetPool<ComponentAnimationRotateTargetFace>().Add(entityId).TargetFace = actionAnimationRotate.Face;

                if (actionAnimationRotate.DelayMSec > 0)
                {
                    world.GetPool<ComponentAnimationDelay>().Add(entityId).DelayMSec = actionAnimationRotate.DelayMSec + dt;
                }
                
                return true;
            }
            
            return false;
        }
    }
}