using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.DragDrop.Parameters;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Places;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Play.DragDrop.OnDrag
{
    public interface ISystemOnDrag : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }

    public sealed class SystemOnDrag : ISystemOnDrag
    {
        private JObject _uiEventData;
        private EcsFilter _placesFilter;
        private EcsFilter _dragDropParameterFilter;
        
        public static ISystemOnDrag Create()
        {
            return new SystemOnDrag();
        }
        
        private SystemOnDrag() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(UiEvents.UiDragEvent, this);

            var world = systems.GetWorld();
            _placesFilter = world.Filter<ComponentPlaceTag>()
                .Inc<ComponentPlaceId>()
                .Inc<ComponentPlaceDragDropId>()
                .End();
            _dragDropParameterFilter = world.Filter<ComponentDragDropParametersTag>()
                .Inc<ComponentDragDropParametersId>()
                .Inc<ComponentDragDropParametersDestinations>()
                .Inc<ComponentDragDropParametersDestinationCondition>()
                .Inc<ComponentDragDropParametersRequiredEclipseCardType>()
                .End();
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(UiEvents.UiDragEvent, this);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }

            var world = systems.GetWorld();
            var dragDropTagPool = world.GetPool<ComponentDragDropTag>();
            var dragDropSourcePlaceIdPool = world.GetPool<ComponentDragDropSourcePlaceId>();
            var dragDropEclipseCarTypePool = world.GetPool<ComponentDragDropEclipseCardType>();

            if (_uiEventData.TryGetValue("entity_id", out int entityId) 
                && dragDropTagPool.Has(entityId)
                && dragDropSourcePlaceIdPool.Has(entityId)
                && dragDropEclipseCarTypePool.Has(entityId))
            {
                var sourcePlaceId = dragDropSourcePlaceIdPool.Get(entityId).SourcePlaceId;
                var eclipseCardType = dragDropEclipseCarTypePool.Get(entityId).CardType;

                foreach (var placeEntityId in _placesFilter)
                {
                    if (world.GetPool<ComponentPlaceId>().Get(placeEntityId).Id != sourcePlaceId)
                    {
                        continue;
                    }

                    var placeDragDropId = world.GetPool<ComponentPlaceDragDropId>().Get(placeEntityId).DragDropId;
                    foreach (var dragDropParameterEntityId in _dragDropParameterFilter)
                    {
                        if (world.GetPool<ComponentDragDropParametersId>().Get(dragDropParameterEntityId).Id !=
                            placeDragDropId)
                        {
                            continue;
                        }

                        var requiredEclipseCardType =
                            world.GetPool<ComponentDragDropParametersRequiredEclipseCardType>()
                                .Get(dragDropParameterEntityId).RequiredEclipseCardType;

                        if (requiredEclipseCardType != eclipseCardType)
                        {
                            break;
                        }
                        
                        Debug.Log("On drag!");
                        
                        break;
                    }
                    
                    break;
                }
            }
            
            _uiEventData = null;
        }

        void IEventListener.OnEvent(string eventKey, object eventData)
        {
            if (eventKey == UiEvents.UiDragEvent && eventData is JObject ed)
            {
                _uiEventData = ed;
            }
        }
    }
}