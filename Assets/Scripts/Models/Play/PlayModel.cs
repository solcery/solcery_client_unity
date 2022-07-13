using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games;
using Solcery.Games.States.New;
using Solcery.Models.Play.Click;
using Solcery.Models.Play.DragDrop.OnDrag;
using Solcery.Models.Play.DragDrop.OnDragMove;
using Solcery.Models.Play.DragDrop.OnDrop;
using Solcery.Models.Play.DragDrop.Parameters;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Play.Initial.Game.Content;
using Solcery.Models.Play.Places;
using Solcery.Models.Play.Timer;
using Solcery.Models.Play.Tooltip;
using Solcery.Models.Shared.Initial.Game.Content;

namespace Solcery.Models.Play
{
    public sealed class PlayModel : IPlayModel
    {
        public EcsWorld World { get; private set; }
        
        private EcsSystems _systems;
        private IUpdateStateQueue _updateStateQueue;
        private bool _isInit;

        public static IPlayModel Create()
        {
            return new PlayModel();
        }

        private PlayModel() { }
        
        void IPlayModel.Init(IGame game, JObject gameContentJson)
        {
            World = new EcsWorld();
            _systems = new EcsSystems(World);
            
            // TODO: чистые инициализационные системы, вызываются один раз, по порядку (важно!)
            _systems.Add(SystemInitialDragDropTypes.Create(game.GameContent));
            _systems.Add(SystemInitialGameContentPlaces.Create(game.GameContent));
            _systems.Add(SystemPlaceInitVisibility.Create(game.GameContent));
            _systems.Add(SystemInitialGameContentPlaceWidgets.Create(game));
            _systems.Add(SystemInitialGameContentEntityTypes.Create(game.GameContent));
            _systems.Add(SystemInitialGameContentTooltips.Create(game.GameContent));

            // TODO первым делом проверяем наличие нового game state
            _systems.Add(SystemGameStateUpdate.Create(game));
            
            // TODO сюда добавляем новые системы и тд
            _systems.Add(SystemPlaceUpdateVisibility.Create(game.ServiceBricks as IServiceBricksInternal));
            _systems.Add(SystemPlaceUpdateWidgets.Create(game));
            
            // TODO обновляем таймер
            _systems.Add(SystemTimerUpdate.Create(game));
            
            // TODO drag drop
            _systems.Add(SystemOnDrag.Create(game));
            _systems.Add(SystemOnDragMove.Create());
            _systems.Add(SystemOnDrop.Create(game));
            
            // TODO clicks
            _systems.Add(SystemOnLeftClick.Create(game));
            _systems.Add(SystemOnRightClick.Create(game));
            
            // TODO tooltip
            _systems.Add(SystemOnTooltipShow.Create());
            _systems.Add(SystemOnTooltipHide.Create());
            

#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif
            
            // TODO Система удаляет компонет в конце цикла, возможно стоит вынести в отдельные подсистемы
            _systems.Add(SystemGameStateRemoveUpdateTag.Create());

            _systems.Init();

            _updateStateQueue = game.UpdateStateQueue;
            _isInit = true;
        }

        void IPlayModel.Update(float dt)
        {
            if (!_isInit)
            {
                return;    
            }
            
            _updateStateQueue.Update((int)(dt * 1000f));
            _systems.Run();
        }
        
        void IPlayModel.Destroy()
        {
            _isInit = false;
            
            _systems?.Destroy();
            _systems = null;
            
            World?.Destroy();
            World = null;

            _updateStateQueue = null;
        }
    }
}