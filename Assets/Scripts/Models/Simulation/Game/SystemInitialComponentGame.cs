using Leopotam.EcsLite;
using Solcery.Games;

namespace Solcery.Models.Simulation.Game
{
    public interface ISystemInitialComponentGame : IEcsInitSystem
    {
    }

    public sealed class SystemInitialComponentGame : ISystemInitialComponentGame
    {
        private IGame _game;

        public static ISystemInitialComponentGame Create(IGame game)
        {
            return new SystemInitialComponentGame(game);
        }

        private SystemInitialComponentGame(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            world.GetPool<ComponentGame>().Add(world.NewEntity()).Game = _game;
            _game = null;
        }
    }
}