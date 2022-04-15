using Leopotam.EcsLite;
using Solcery.Models.Shared.Objects;

namespace Solcery.Models.Simulation.Game.Destroy
{
    public interface ISystemDestroyObjects : IEcsInitSystem, IEcsRunSystem
    {
    }

    public sealed class SystemDestroyObjects : ISystemDestroyObjects
    {
        private EcsFilter _filterDestroyedObjects;
        
        public static ISystemDestroyObjects Create()
        {
            return new SystemDestroyObjects();
        }

        private SystemDestroyObjects() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterDestroyedObjects = systems.GetWorld().Filter<ComponentObjectDeletedTag>().End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            foreach (var entityId in _filterDestroyedObjects)
            {
                world.DelEntity(entityId);
            }
        }
    }
}