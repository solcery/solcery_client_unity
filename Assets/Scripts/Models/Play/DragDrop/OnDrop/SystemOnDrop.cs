using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Play.DragDrop.Parameters;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Commands.Datas.OnDrop;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Services.Events;
using Solcery.Widgets_new;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;
using UnityEngine.EventSystems;

namespace Solcery.Models.Play.DragDrop.OnDrop
{
    public interface ISystemOnDrop : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemOnDrop : ISystemOnDrop
    {
        private IGame _game;
        private EventData _uiEventData;
        
        public static ISystemOnDrop Create(IGame game)
        {
            return new SystemOnDrop(game);
        }

        private SystemOnDrop(IGame game)
        {
            _game = game;
        }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnDropEventData.OnDropEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnDropEventData.OnDropEventName, this);
        }
        
        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnDropEventData.OnDropEventName)
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
            var viewPool = world.GetPool<ComponentDragDropView>();
            var sourcePlaceEntityIdPool = world.GetPool<ComponentDragDropSourcePlaceEntityId>();

            if (_uiEventData is OnDropEventData onDropEventData 
                && viewPool.Has(onDropEventData.DragEntityId))
            {
                var raycastResult = new List<RaycastResult>();
                EventSystem.current.RaycastAll(onDropEventData.PointerEventData, raycastResult);
                var sourcePlaceEntityId = sourcePlaceEntityIdPool.Get(onDropEventData.DragEntityId).SourcePlaceEntityId;
                PlaceWidgetLayout targetLayout = null;
                IApplyDropWidget targetDropWidget = null;

                if (raycastResult.Count > 0)
                {
                    TryGetRaycastLayout(raycastResult[0], out targetLayout);
                }

                if (targetLayout != null
                    && targetLayout.LinkedEntityId != sourcePlaceEntityId
                    && CheckPlaceDestinations(world, onDropEventData.DragDropEntityId, targetLayout.PlaceId)
                    && CheckDestinationCondition(world, onDropEventData.DragDropEntityId, targetLayout.PlaceId)
                    && TryGetTargetDropWidget(world, targetLayout.LinkedEntityId, out targetDropWidget))
                {
                    sourcePlaceEntityIdPool.Get(onDropEventData.DragEntityId).SourcePlaceEntityId =
                        targetLayout.LinkedEntityId;
                    var objectId = world.GetPool<ComponentDragDropObjectId>().Get(onDropEventData.DragEntityId).ObjectId;
                    var dragDropId = world.GetPool<ComponentDragDropParametersId>().Get(onDropEventData.DragDropEntityId).Id;
                    var command = CommandOnDropData.CreateFromParameters(objectId, dragDropId, targetLayout.PlaceId,
                            TriggerTargetEntityTypes.Card);
                    _game.TransportService.SendCommand(command.ToJson());
                }

                viewPool.Get(onDropEventData.DragEntityId).View.OnDrop(onDropEventData.WorldPosition, targetDropWidget);
            }
            
            _uiEventData = null;
        }

        private bool TryGetRaycastLayout(RaycastResult raycastResult, out PlaceWidgetLayout placeWidgetLayout)
        {
            placeWidgetLayout = null;
            
            if (raycastResult.gameObject == null)
            {
                return false;
            }

            placeWidgetLayout = raycastResult.gameObject.GetComponentInParent<PlaceWidgetLayout>();
            return placeWidgetLayout != null;
        }

        private bool CheckPlaceDestinations(EcsWorld world, int dragDropEntityId, int targetPlaceId)
        {
            var dragDropParameterDestinationPool = world.GetPool<ComponentDragDropParametersDestinations>();
            return dragDropParameterDestinationPool.Has(dragDropEntityId) && dragDropParameterDestinationPool
                .Get(dragDropEntityId).PlaceIds.Contains(targetPlaceId);
        }

        private bool TryGetTargetDropWidget(EcsWorld world, int targetPlaceEntityId, out IApplyDropWidget targetDropWidget)
        {
            targetDropWidget = null;
            
            var placeWidgetPool = world.GetPool<ComponentPlaceWidgetNew>();
            if (placeWidgetPool.Has(targetPlaceEntityId) 
                && placeWidgetPool.Get(targetPlaceEntityId).Widget is IApplyDropWidget adw)
            {
                targetDropWidget = adw;
            }

            return targetDropWidget != null;
        }

        private bool CheckDestinationCondition(EcsWorld world, int dragDropEntityId, int targetPlaceId)
        {
            var destinationConditionPool = world.GetPool<ComponentDragDropParametersDestinationCondition>();

            if (destinationConditionPool.Has(dragDropEntityId))
            {
                var destinationConditionType =
                    destinationConditionPool.Get(dragDropEntityId).ParametersDestinationConditionType;

                var filter = world.Filter<ComponentObjectTag>().Inc<ComponentObjectAttributes>().End();
                var objectAttributesPool = world.GetPool<ComponentObjectAttributes>();
                var countObjectOnPlace = 0;

                foreach (var entityId in filter)
                {
                    if (objectAttributesPool.Get(entityId).Attributes.TryGetValue("place", out var placeId) 
                        && placeId.Current == targetPlaceId)
                    {
                        countObjectOnPlace++;
                    }
                }

                switch (destinationConditionType)
                {
                    case DragDropParametersDestinationConditionTypes.Empty:
                        return countObjectOnPlace <= 0;
                    
                    case DragDropParametersDestinationConditionTypes.NonEmpty:
                        return countObjectOnPlace > 0;
                }
            }
            
            return false;
        }
    }
}