using Leopotam.EcsLite;
using Solcery.Models.Play.Actions.Animation;
using Solcery.Models.Play.Actions.Animation.Visibility;
using UnityEngine;

namespace Solcery.Models.Play.Animation.Visibility
{
    public interface ISystemAnimationVisibility : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemAnimationVisibility : ISystemAnimationVisibility
    {
        private EcsFilter _filter;
        
        public static ISystemAnimationVisibility Create()
        {
            return new SystemAnimationVisibility();
        }

        private SystemAnimationVisibility() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            _filter = systems.GetWorld()
                .Filter<ComponentAnimationVisibilityTag>()
                .Inc<ComponentAnimationObjectId>()
                .Inc<ComponentAnimationVisibilityVisible>()
                .Exc<ComponentAnimationDelay>()
                .End();
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var objectIdPool = world.GetPool<ComponentAnimationObjectId>();
            var visiblePool = world.GetPool<ComponentAnimationVisibilityVisible>();

            foreach (var entityId in _filter)
            {
                var objectId = objectIdPool.Get(entityId).ObjectId;
                var visible = visiblePool.Get(entityId).Visible;
                
                Debug.Log($"Play visible animation objId {objectId} visible {visible}");
                
                world.DelEntity(entityId);
            }
        }
    }
}