using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Attributes.Highlighted;
using Solcery.Models.Play.Attributes.Interactable;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Play.Initial.Game.Content;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Initial.Game.Content;

namespace Solcery.Models.Play
{
    public sealed class PlayPlayModel : IPlayModel
    {
        public EcsWorld World { get; private set; }
        
        private EcsSystems _systems;

        public static IPlayModel Create()
        {
            return new PlayPlayModel();
        }

        private PlayPlayModel() { }
        
        void IPlayModel.Init(IGame game, JObject gameContentJson)
        {
            World = new EcsWorld();
            _systems = new EcsSystems(World);
            
            // TODO: чистые инициализационные системы, вызываются один раз, по порядку (важно!)
            _systems.Add(SystemInitialGameContentPlaces.Create(game.GameContent));
            _systems.Add(SystemInitialGameContentPlaceWidgets.Create(game));
            _systems.Add(SystemInitialGameContentEntityTypes.Create(game.GameContent));
            
            // TODO первым делом проверяем наличие нового game state
            _systems.Add(SystemGameStateUpdate.Create(game));
            
            _systems.Add(SystemPlaceWidgetsUpdate.Create());
            _systems.Add(SystemApplyAttributeInteractable.Create());
            _systems.Add(SystemApplyAttributeHighlighted.Create());
            // TODO сюда добавляем новые системы и тд
            
#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif
            
            // TODO Система удаляет компонет в конце цикла, возможно стоит вынести в отдельные подсистемы
            _systems.Add(SystemGameStateRemoveUpdateTag.Create());
            
            

            _systems.Init();
        }

        void IPlayModel.Update(float dt)
        {
            _systems?.Run();
        }
        
        void IPlayModel.Destroy()
        {
            _systems?.Destroy();
            _systems = null;
            
            World?.Destroy();
            World = null;
        }
    }
}