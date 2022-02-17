using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Places;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;

namespace Solcery.Models.Play.DragDrop.OnDrag
{
    public interface ISystemOnDrag : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }

    public sealed class SystemOnDrag : ISystemOnDrag
    {
        private JObject _uiEventData;
        private EcsFilter _placesFilter;
        
        public static ISystemOnDrag Create()
        {
            return new SystemOnDrag();
        }
        
        private SystemOnDrag() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(UiEvents.UiDragEvent, this);

            var world = systems.GetWorld();
            _placesFilter = world.Filter<ComponentPlaceTag>().Exc<ComponentPlaceId>().End();
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

            if (_uiEventData.TryGetValue("entity_id", out int entityId) 
                && dragDropTagPool.Has(entityId)
                && dragDropSourcePlaceIdPool.Has(entityId))
            {
                var sourcePlaceId = dragDropSourcePlaceIdPool.Get(entityId).SourcePlaceId;
                
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