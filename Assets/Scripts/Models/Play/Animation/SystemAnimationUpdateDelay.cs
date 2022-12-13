using Leopotam.EcsLite;
using Solcery.Models.Play.Actions.Animation;
using UnityEngine;

namespace Solcery.Models.Play.Animation
{
    public interface ISystemAnimationUpdateDelay : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemAnimationUpdateDelay : ISystemAnimationUpdateDelay
    {
        private EcsFilter _filter;
        
        public static ISystemAnimationUpdateDelay Create()
        {
            return new SystemAnimationUpdateDelay();
        }
        
        private SystemAnimationUpdateDelay() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            _filter = systems.GetWorld()
                .Filter<ComponentAnimationDelay>()
                .End();
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var delayPool = world.GetPool<ComponentAnimationDelay>();
            var durationPool = world.GetPool<ComponentAnimationDuration>();
            var dt = (int)(Time.deltaTime * 1000);
            
            foreach (var entityId in _filter)
            {
                var delayMSec = delayPool.Get(entityId).DelayMSec;
                delayMSec -= dt;
                
                if (delayMSec <= 0)
                {
                    if (durationPool.Has(entityId))
                    {
                        var durationMSec = durationPool.Get(entityId).DurationMSec;
                        durationMSec += delayMSec;
                        durationPool.Get(entityId).DurationMSec = durationMSec;
                    }
                    
                    delayPool.Del(entityId);
                    continue;
                }
                
                delayPool.Get(entityId).DelayMSec = delayMSec;
            }
        }
    }
}