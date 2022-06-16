using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Shared.Commands.Datas.OnClick;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
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
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnLeftClickEventData.OnLeftClickEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnLeftClickEventData.OnLeftClickEventName, this);
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            if (_uiEventData is OnLeftClickEventData eventData)
            {
                OnLeftClick(systems.GetWorld(), eventData.EntityId);
            }
            
            _uiEventData = null;
        }

        private void OnLeftClick(EcsWorld world, int entityId)
        {
            var objectId = world.GetPool<ComponentObjectId>().Get(entityId).Id;
            var command = CommandOnLeftClickData.CreateFromParameters(objectId, TriggerTargetEntityTypes.Card);
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