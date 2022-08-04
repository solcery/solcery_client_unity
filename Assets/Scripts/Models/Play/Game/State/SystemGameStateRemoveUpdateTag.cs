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
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            _filterGameStateUpdateTag = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            foreach (var index in _filterGameStateUpdateTag)
            {
                systems.GetWorld().DelEntity(index);
            }
        }
    }
}