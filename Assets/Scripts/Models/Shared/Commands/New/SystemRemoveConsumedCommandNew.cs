using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Commands.New
{
    public interface ISystemRemoveConsumedCommandNew : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemRemoveConsumedCommandNew : ISystemRemoveConsumedCommandNew
    {
        private EcsFilter _filterConsumedCommands;
        
        public static ISystemRemoveConsumedCommandNew Create()
        {
            return new SystemRemoveConsumedCommandNew();
        }
        
        private SystemRemoveConsumedCommandNew() {}
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterConsumedCommands = world.Filter<ComponentCommandTag>().Inc<ComponentCommandConsumeTag>().End();
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            foreach (var entityId in _filterConsumedCommands)
            {
                world.DelEntity(entityId);
            }
        }

        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            _filterConsumedCommands = null;
        }
    }
}