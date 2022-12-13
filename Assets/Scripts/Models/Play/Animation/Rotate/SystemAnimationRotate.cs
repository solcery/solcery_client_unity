using Leopotam.EcsLite;
using Solcery.Models.Play.Actions.Animation;
using Solcery.Models.Play.Actions.Animation.Rotate;
using UnityEngine;

namespace Solcery.Models.Play.Animation.Rotate
{
    public interface ISystemAnimationRotate : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemAnimationRotate : ISystemAnimationRotate
    {
        private EcsFilter _filter;

        public static ISystemAnimationRotate Create()
        {
            return new SystemAnimationRotate();
        }
        
        private SystemAnimationRotate() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            _filter = systems.GetWorld()
                .Filter<ComponentAnimationRotateTag>()
                .Inc<ComponentAnimationObjectId>()
                .Inc<ComponentAnimationDuration>()
                .Inc<ComponentAnimationRotateTargetFace>()
                .Exc<ComponentAnimationDelay>()
                .End();
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var objectIdPool = world.GetPool<ComponentAnimationObjectId>();
            var durationPool = world.GetPool<ComponentAnimationDuration>();
            var targetFacePool = world.GetPool<ComponentAnimationRotateTargetFace>();
            
            foreach (var entityId in _filter)
            {
                var objectId = objectIdPool.Get(entityId).ObjectId;
                var durationMSec = durationPool.Get(entityId).DurationMSec;
                var targetFace = targetFacePool.Get(entityId).TargetFace;
                
                Debug.Log($"Play rotate animation objId {objectId} duration {durationMSec} terget face {targetFace}");
                
                world.DelEntity(entityId);
            }
        }
    }
}