using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Play.DragDrop.Parameters;
using Solcery.Models.Play.Places;
using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;

namespace Solcery.Models.Play.DragDrop.OnDrag
{
    public interface ISystemOnDrag : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemOnDrag : ISystemOnDrag
    {
        private IGame _game;
        private EventData _uiEventData;
        
        public static ISystemOnDrag Create(IGame game)
        {
            return new SystemOnDrag(game);
        }

        private SystemOnDrag(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnDragEventData.OnDragEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnDragEventData.OnDragEventName, this);
        }
        
        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnDragEventData.OnDragEventName)
            {
                _uiEventData = eventData;
            }
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            var world = systems.GetWorld();
            var dragDropTagPool = world.GetPool<ComponentDragDropTag>();
            var dragDropSourcePlaceEntityIdPool = world.GetPool<ComponentDragDropSourcePlaceEntityId>();
            var dragDropEclipseCarTypePool = world.GetPool<ComponentDragDropEclipseCardType>();

            if (_uiEventData is OnDragEventData onDragEventData
                && dragDropTagPool.Has(onDragEventData.DragEntityId)
                && dragDropSourcePlaceEntityIdPool.Has(onDragEventData.DragEntityId)
                && dragDropEclipseCarTypePool.Has(onDragEventData.DragEntityId))
            {
                var placeEntityId = dragDropSourcePlaceEntityIdPool.Get(onDragEventData.DragEntityId).SourcePlaceEntityId;
                var eclipseCardType = dragDropEclipseCarTypePool.Get(onDragEventData.DragEntityId).CardType;

                var dragDropParameterEntityId = world.GetPool<ComponentPlaceDragDropEntityId>().Get(placeEntityId).DragDropEntityId;
                var requiredEclipseCardType =
                    world.GetPool<ComponentDragDropParametersRequiredEclipseCardType>()
                        .Get(dragDropParameterEntityId).RequiredEclipseCardType;

                if (requiredEclipseCardType == eclipseCardType)
                {
                    world.GetPool<ComponentDragDropView>().Get(onDragEventData.DragEntityId).View
                        .OnDrag(_game.WidgetCanvas.GetDragDropCanvas().GetRectTransform, onDragEventData.WorldPosition);
                    _game.WidgetCanvas.GetDragDropCanvas().UpdateOnDrag(onDragEventData.DragEntityId);
                }
            }
            
            _uiEventData = null;
        }
    }
}