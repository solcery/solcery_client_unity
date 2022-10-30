using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Shared.Commands.New;
using Solcery.Models.Shared.Commands.New.OnClick;
using Solcery.Models.Shared.Objects;
using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;

namespace Solcery.Models.Play.Click
{
    public interface ISystemOnLeftClick: IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemOnLeftClick : ISystemOnLeftClick
    {
        private readonly IGame _game;
        private EventData _uiEventData;

        public static SystemOnLeftClick Create(IGame game)
        {
            return new SystemOnLeftClick(game);
        }

        private SystemOnLeftClick(IGame game)
        {
            _game = game;
        }     
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnLeftClickEventData.OnLeftClickEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnLeftClickEventData.OnLeftClickEventName, this);
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            var game = systems.GetShared<IGame>();
            
            if (_uiEventData is OnLeftClickEventData eventData)
            {
                OnLeftClick(game, systems.GetWorld(), eventData.EntityId);
            }
            
            _uiEventData = null;
        }

        private void OnLeftClick(IGame game, EcsWorld world, int entityId)
        {
            var objectId = world.GetPool<ComponentObjectId>().Get(entityId).Id;
            var command =
                CommandOnLeftClickDataNew.Create(game.ServiceGameContent.CommandIdForType(CommandTypesNew.OnLeftClick),
                    game.PlayerIndex, objectId);
            _game.TransportService.SendCommand(command.ToJson());
        }

        public void OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnLeftClickEventData.OnLeftClickEventName)
            {
                _uiEventData = eventData;
            }
        }
    }
}