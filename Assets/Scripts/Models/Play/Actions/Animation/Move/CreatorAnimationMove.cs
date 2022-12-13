using Leopotam.EcsLite;
using Solcery.Games.States.New.Actions;
using Solcery.Games.States.New.Actions.Animation.Move;
using Solcery.Models.Play.Actions.Factory;
using UnityEngine;

namespace Solcery.Models.Play.Actions.Animation.Move
{
    public static class CreatorAnimationMove
    {
        public static bool Create(IActionObjectFactory factory, EcsWorld world, UpdateAction action)
        {
            if (action is UpdateActionAnimationMove actionAnimationMove)
            {
                var dt = (int)(Time.deltaTime * 1000);
                var entityId = world.NewEntity();
                world.GetPool<ComponentAnimationMoveTag>().Add(entityId);
                world.GetPool<ComponentAnimationObjectId>().Add(entityId).ObjectId = actionAnimationMove.ObjectId;
                world.GetPool<ComponentAnimationDuration>().Add(entityId).DurationMSec = actionAnimationMove.DurationMsec;
                world.GetPool<ComponentAnimationMoveFromPlaceId>().Add(entityId).FromPlaceId =
                    actionAnimationMove.FromPlaceId;
                world.GetPool<ComponentAnimationMoveFace>().Add(entityId).Face = actionAnimationMove.Face;

                if (actionAnimationMove.DelayMSec > 0)
                {
                    world.GetPool<ComponentAnimationDelay>().Add(entityId).DelayMSec = actionAnimationMove.DelayMSec + dt;
                }
                
                return true;
            }
            
            return false;
        }
    }
}