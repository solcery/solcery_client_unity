using Leopotam.EcsLite;
using Solcery.Games.States.New.Actions;
using Solcery.Games.States.New.Actions.Animation.Visibility;
using Solcery.Models.Play.Actions.Factory;
using UnityEngine;

namespace Solcery.Models.Play.Actions.Animation.Visibility
{
    public static class CreatorAnimationVisibility
    {
        public static bool Create(IActionObjectFactory factory, EcsWorld world, UpdateAction action)
        {
            if (action is UpdateActionAnimationVisibility actionAnimationVisibility)
            {
                var dt = (int)(Time.deltaTime * 1000);
                var entityId = world.NewEntity();
                world.GetPool<ComponentAnimationVisibilityTag>().Add(entityId);
                world.GetPool<ComponentAnimationObjectId>().Add(entityId).ObjectId = actionAnimationVisibility.ObjectId;
                world.GetPool<ComponentAnimationVisibilityVisible>().Add(entityId).Visible = actionAnimationVisibility.Visible;

                if (actionAnimationVisibility.DelayMSec > 0)
                {
                    world.GetPool<ComponentAnimationDelay>().Add(entityId).DelayMSec = actionAnimationVisibility.DelayMSec + dt;
                }
                
                return true;
            }
            
            return false;
        }
    }
}