using Leopotam.EcsLite;
using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;
using UnityEngine;

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

        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnDragMoveEventData.OnDragMoveEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnDragMoveEventData.OnDragMoveEventName, this);
        }
        
        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnDragMoveEventData.OnDragMoveEventName)
            {
                _uiEventData = eventData;
            }
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
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
                //Debug.Log($"SystemOnDragMove world position {onDragMoveEventData.WorldPosition}");
                viewPool.Get(onDragMoveEventData.DragEntityId).View.OnMove(onDragMoveEventData.WorldPosition);
            }
            
            _uiEventData = null;
        }
    }
}