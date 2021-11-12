using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.GameState;
using Solcery.Models.Places;
using Solcery.Models.Triggers;

namespace Solcery.Models
{
    public sealed class PlayModel : IModel
    {
        public EcsWorld World { get; private set; }
        
        private EcsSystems _systems;

        public static IModel Create()
        {
            return new PlayModel();
        }

        private PlayModel() { }
        
        void IModel.Init(IGame game)
        {
            World = new EcsWorld();

            _systems = new EcsSystems(World);
            _systems.Add(SystemPlaceWidgetsUpdate.Create());
            _systems.Add(SystemApplyTrigger.Create(game.BrickService));
            // TODO сюда добавляем новые системы и тд
            
#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif
            
            // TODO Система удаляет компонет в конце цикла, возможно стоит вынести в отдельные подсистемы
            _systems.Add(SystemGameStateRemoveUpdateTag.Create());

            _systems.Init();
        }

        void IModel.Update(float dt)
        {
            _systems?.Run();
        }
        
        void IModel.Destroy()
        {
            _systems?.Destroy();
            _systems = null;
            
            World?.Destroy();
            World = null;
        }
    }
}