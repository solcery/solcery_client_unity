using Leopotam.EcsLite;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;

namespace Solcery.Models.Play.DragDrop.OnDragMove
{
    public interface ISystemOnDragMove : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemOnDragMove : ISystemOnDragMove
    {
        private EventData _uiEventData;
        
        public static ISystemOnDragMove Create()
        {
            return new SystemOnDragMove();
        }

        private SystemOnDragMove() { }
        
        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == UiEvents.UiDragMoveEvent)
            {
                _uiEventData = eventData;
            }
        }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(UiEvents.UiDragMoveEvent, this);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }

            var world = systems.GetWorld();
            var viewPool = world.GetPool<ComponentDragDropView>();
            
            if (_uiEventData is OnDragMoveEventData onDragMoveEventData
                && viewPool.Has(onDragMoveEventData.DragEntityId))
            {
                viewPool.Get(onDragMoveEventData.DragEntityId).View.OnMove(onDragMoveEventData.WorldPosition);
            }
            
            _uiEventData = null;
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(UiEvents.UiDragMoveEvent, this);
        }
    }
}