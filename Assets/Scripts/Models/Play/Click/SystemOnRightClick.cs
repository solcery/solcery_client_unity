using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;

namespace Solcery.Models.Play.Click
{
    public interface ISystemOnRightClick: IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemOnRightClick : ISystemOnRightClick
    {
        private readonly IGame _game;
        private EventData _uiEventData;

        public static SystemOnRightClick Create(IGame game)
        {
            return new SystemOnRightClick(game);
        }

        private SystemOnRightClick(IGame game)
        {
            _game = game;
        }     
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnRightClickEventData.OnRightClickEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnRightClickEventData.OnRightClickEventName, this);
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            if (_uiEventData is OnRightClickEventData eventData)
            {
                OnRightClick(systems.GetWorld(), eventData.EntityId);
            }
            
            _uiEventData = null;
        }

        private void OnRightClick(EcsWorld world, int entityId)
        {
            // var objectId = world.GetPool<ComponentObjectId>().Get(entityId).Id;
            // var command = CommandOnRightClickData.CreateFromParameters(objectId, TriggerTargetEntityTypes.Card);
            // _game.TransportService.SendCommand(command.ToJson());
        }

        public void OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnRightClickEventData.OnRightClickEventName)
            {
                _uiEventData = eventData;
            }
        }
    }
}