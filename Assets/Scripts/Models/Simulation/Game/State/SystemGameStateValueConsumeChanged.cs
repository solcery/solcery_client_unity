using Leopotam.EcsLite;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Objects;

namespace Solcery.Models.Simulation.Game.State
{
    public interface ISystemGameStateValueConsumeChanged : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemGameStateValueConsumeChanged : ISystemGameStateValueConsumeChanged
    {
        private EcsFilter _filterGameAttributes;
        private EcsFilter _filterGameEntities;
        
        public static ISystemGameStateValueConsumeChanged Create()
        {
            return new SystemGameStateValueConsumeChanged();
        }
        
        private SystemGameStateValueConsumeChanged() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            _filterGameAttributes = world.Filter<ComponentGameAttributes>()
                .End();
            
            _filterGameEntities = world.Filter<ComponentObjectTag>()
                .Inc<ComponentObjectAttributes>()
                .Exc<ComponentObjectDeletedTag>()
                .End();
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            // Consume changing in game attributes
            {
                var gameAttrsPool = world.GetPool<ComponentGameAttributes>();
                foreach (var entity in _filterGameAttributes)
                {
                    ref var attrs = ref gameAttrsPool.Get(entity);
                    foreach (var attr in attrs.Attributes)
                    {
                        attr.Value.ConsumeChanged();
                    }
                }
            }

            // Consume changing in objects attributes
            {
                var attrsPool = world.GetPool<ComponentObjectAttributes>();
                foreach (var entity in _filterGameEntities)
                {
                    ref var attrs = ref attrsPool.Get(entity);
                    foreach (var attr in attrs.Attributes)
                    {
                        attr.Value.ConsumeChanged();
                    }
                }
            }
        }
    }
}