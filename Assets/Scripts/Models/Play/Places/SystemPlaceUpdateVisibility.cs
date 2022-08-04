using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games;
using Solcery.Games.Contexts;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Shared.Places;

namespace Solcery.Models.Play.Places
{
    public interface ISystemPlaceUpdateVisibility : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }

    public sealed class SystemPlaceUpdateVisibility : ISystemPlaceUpdateVisibility
    {
        private EcsFilter _filterPlaceVisibilityBrick;
        private EcsFilter _filterGameStateUpdate;
        private IServiceBricksInternal _serviceBricks;

        public static ISystemPlaceUpdateVisibility Create(IServiceBricksInternal serviceBricks)
        {
            return new SystemPlaceUpdateVisibility(serviceBricks);
        }

        private SystemPlaceUpdateVisibility(IServiceBricksInternal serviceBricks)
        {
            _serviceBricks = serviceBricks;
        }

        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterPlaceVisibilityBrick = world.Filter<ComponentPlaceTag>()
                .Inc<ComponentPlaceVisibilityBrick>()
                .Inc<ComponentPlaceIsVisible>()
                .End();
            _filterGameStateUpdate = world.Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            _serviceBricks = null;
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }

            var game = systems.GetShared<IGame>();
            var world = systems.GetWorld();
            var poolPlaceVisible = world.GetPool<ComponentPlaceIsVisible>();
            var poolPlaceVisibilityBrick = world.GetPool<ComponentPlaceVisibilityBrick>();
            foreach (var placeEntityId in _filterPlaceVisibilityBrick)
            {
                ref var placeIsVisible = ref poolPlaceVisible.Get(placeEntityId);
                var placeVisibilityBrick = poolPlaceVisibilityBrick.Get(placeEntityId).VisibilityBrick;
                
                var context = CurrentContext.Create(game, world);
                placeIsVisible.IsVisible = placeVisibilityBrick != null 
                                           && _serviceBricks.ExecuteConditionBrick(placeVisibilityBrick, context, 1, out var result) 
                                           && result;
                CurrentContext.Destroy(world, context);
            }
        }
    }
}