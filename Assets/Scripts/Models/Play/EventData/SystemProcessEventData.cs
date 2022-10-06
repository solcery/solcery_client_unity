using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Play.DragDrop;
using Solcery.Models.Play.Places;
using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;

namespace Solcery.Models.Play
{
    public interface ISystemProcessEventData : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }
    
    public class SystemProcessEventData : ISystemProcessEventData
    {
        private IGame _game;
        private EventData _uiEventData;
        
        public static ISystemProcessEventData Create(IGame game)
        {
            return new SystemProcessEventData(game);
        }

        private SystemProcessEventData(IGame game)
        {
            _game = game;
        }
        
        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnUniversalEventData.OnUniversalEventDataName)
            {
                _uiEventData = eventData;
            }
        }

        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnUniversalEventData.OnUniversalEventDataName, this);
        }
        
        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnUniversalEventData.OnUniversalEventDataName, this);
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            var world = systems.GetWorld();
            var dragDropSourcePlaceEntityIdPool = world.GetPool<ComponentDragDropSourcePlaceEntityId>();
            var placeDragDropEntityIdPool = world.GetPool<ComponentPlaceDragDropEntityId>();
            var sendOnDragEvent = false;

            if (_uiEventData is OnUniversalEventData eventData)
            {
                if (dragDropSourcePlaceEntityIdPool.Has(eventData.DragEntityId))
                {
                    var placeEntityId = dragDropSourcePlaceEntityIdPool.Get(eventData.DragEntityId).SourcePlaceEntityId;
                    
                    
                    if (placeDragDropEntityIdPool.Has(placeEntityId))
                    {
                        var dragDropEntityIds = placeDragDropEntityIdPool.Get(placeEntityId).DragDropEntityIds;
                        sendOnDragEvent = dragDropEntityIds.Count > 0;
                    }
                }

                if (sendOnDragEvent)
                {
                    ServiceEvents.Current.BroadcastEvent(OnDragEventData.Create(eventData.DragObjectEntityId, eventData.DragEntityId, eventData.WorldPosition, eventData.PointerEventData));
                }
                else
                {
                    ServiceEvents.Current.BroadcastEvent(OnLeftClickEventData.Create(eventData.DragObjectEntityId));
                }
            }

            _uiEventData = null;
        }
    }
}