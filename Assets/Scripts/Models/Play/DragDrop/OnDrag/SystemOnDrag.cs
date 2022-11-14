using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games;
using Solcery.Games.Contexts;
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
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnDragEventData.OnDragEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(IEcsSystems systems)
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

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            var game = systems.GetShared<IGame>();
            var serviceBricks = game.ServiceBricks as IServiceBricksInternal;
            var world = systems.GetWorld();
            var dragDropTagPool = world.GetPool<ComponentDragDropTag>();
            var dragDropSourcePlaceEntityIdPool = world.GetPool<ComponentDragDropSourcePlaceEntityId>();
            var dragDropEclipseCarTypePool = world.GetPool<ComponentDragDropEclipseCardType>();
            var placeDragDropEntityIdPool = world.GetPool<ComponentPlaceDragDropEntityId>();

            if (_uiEventData is OnDragEventData onDragEventData
                && dragDropTagPool.Has(onDragEventData.DragEntityId)
                && dragDropSourcePlaceEntityIdPool.Has(onDragEventData.DragEntityId)
                && dragDropEclipseCarTypePool.Has(onDragEventData.DragEntityId))
            {
                var placeEntityId = dragDropSourcePlaceEntityIdPool.Get(onDragEventData.DragEntityId).SourcePlaceEntityId;

                if (placeDragDropEntityIdPool.Has(placeEntityId))
                {
                    var dragDropEntityIds = placeDragDropEntityIdPool.Get(placeEntityId).DragDropEntityIds;

                    foreach (var dragDropEntityId in dragDropEntityIds)
                    {
                        var originConditionBrick = world.GetPool<ComponentDragDropBrickOriginCondition>()
                            .Get(dragDropEntityId).ConditionBrick;
                        
                        var context = CurrentContext.Create(game, world, null);
                        context.Object.Push(onDragEventData.DragObjectEntityId);

                        if (originConditionBrick == null 
                            || (serviceBricks != null 
                                && serviceBricks.ExecuteConditionBrick(originConditionBrick, context, 0, out var result)
                                && result))
                        {
                            world.GetPool<ComponentDragDropView>().Get(onDragEventData.DragEntityId).View
                                .OnDrag(_game.WidgetCanvas.GetDragDropCanvas().GetRectTransform,
                                    onDragEventData.WorldPosition);
                            _game.WidgetCanvas.GetDragDropCanvas()
                                .UpdateOnDrag(onDragEventData.DragObjectEntityId, onDragEventData.DragEntityId, dragDropEntityId);
                            CurrentContext.Destroy(world, context);
                            break;
                        }
                        
                        CurrentContext.Destroy(world, context);
                    }
                }
            }
            
            _uiEventData = null;
        }
    }
}