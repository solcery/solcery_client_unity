using Leopotam.EcsLite;

namespace Solcery.Models.Play.Game.State
{
    public interface ISystemGameStateRemoveUpdateTag : IEcsInitSystem, IEcsRunSystem
    {
    }

    public class SystemGameStateRemoveUpdateTag : ISystemGameStateRemoveUpdateTag
    {
        private EcsFilter _filterGameStateUpdateTag;
        
        public static ISystemGameStateRemoveUpdateTag Create()
        {
            return new SystemGameStateRemoveUpdateTag();
        }

        private SystemGameStateRemoveUpdateTag() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterGameStateUpdateTag = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            foreach (var index in _filterGameStateUpdateTag)
            {
                systems.GetWorld().DelEntity(index);
            }
        }
    }
}