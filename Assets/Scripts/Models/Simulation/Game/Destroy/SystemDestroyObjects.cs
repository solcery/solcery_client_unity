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
        private EcsFilter _filterComponentObjectIdHash;
        
        public static ISystemDestroyObjects Create()
        {
            return new SystemDestroyObjects();
        }

        private SystemDestroyObjects() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterDestroyedObjects = systems.GetWorld().Filter<ComponentObjectDeletedTag>().End();
            _filterComponentObjectIdHash = systems.GetWorld().Filter<ComponentObjectIdHash>().End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var objectIdPool = world.GetPool<ComponentObjectId>();

            foreach (var oidEntityId in _filterComponentObjectIdHash)
            {
                var objectIdHashes = world.GetPool<ComponentObjectIdHash>().Get(oidEntityId).ObjectIdHashes;
                
                foreach (var entityId in _filterDestroyedObjects)
                {
                    if (objectIdPool.Has(entityId))
                    {
                        var objId = objectIdPool.Get(entityId).Id;
                        objectIdHashes.ReleaseId(objId);
                    }
                    
                    world.DelEntity(entityId);
                }
                
                break;
            }
        }
    }
}