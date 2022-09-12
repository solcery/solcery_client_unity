using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games;
using Solcery.Games.Contexts;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Shared.Places;

namespace Solcery.Models.Play.Places
{
    public interface ISystemPlaceUpdateAvailability : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemPlaceUpdateAvailability : ISystemPlaceUpdateAvailability
    {
        private EcsFilter _filterPlaceAvailabilityBrick;
        private EcsFilter _filterGameStateUpdate;
        private IServiceBricksInternal _serviceBricks;
        
        public static ISystemPlaceUpdateAvailability Create(IServiceBricksInternal serviceBricks)
        {
            return new SystemPlaceUpdateAvailability(serviceBricks);
        }

        private SystemPlaceUpdateAvailability(IServiceBricksInternal serviceBricks)
        {
            _serviceBricks = serviceBricks;
        }
        
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterPlaceAvailabilityBrick = world.Filter<ComponentPlaceTag>()
                .Inc<ComponentPlaceAvailableBrick>()
                .Inc<ComponentPlaceIsAvailable>()
                .End();
            _filterGameStateUpdate = world.Filter<ComponentGameStateUpdateTag>().End();
        }

        public void Destroy(IEcsSystems systems)
        {
            _serviceBricks = null;
        }
        
        public void Run(IEcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }
            var game = systems.GetShared<IGame>();
            var world = systems.GetWorld();
            var poolPlaceAvailable = world.GetPool<ComponentPlaceIsAvailable>();
            var poolPlaceAvailableBrick = world.GetPool<ComponentPlaceAvailableBrick>();
            foreach (var placeEntityId in _filterPlaceAvailabilityBrick)
            {
                ref var placeIsAvailable = ref poolPlaceAvailable.Get(placeEntityId);
                var placeAvailableBrick = poolPlaceAvailableBrick.Get(placeEntityId).AvailableBrick;
                var context = CurrentContext.Create(game, world);
                if (placeAvailableBrick != null)
                {
                    placeIsAvailable.IsAvailable = _serviceBricks.ExecuteConditionBrick(placeAvailableBrick, context, 1, out var result) 
                                                   && result;
                }
                else
                {
                    placeIsAvailable.IsAvailable = true;
                }

                CurrentContext.Destroy(world, context);
            }
        }
    }
}
