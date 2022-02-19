using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Utils;
using Solcery.Ui;
using UnityEngine;

namespace Solcery.Models.Play.DragDrop.OnDrop
{
    public interface ISystemOnDrop : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemOnDrop : ISystemOnDrop
    {
        private JObject _uiEventData;
        
        public static ISystemOnDrop Create()
        {
            return new SystemOnDrop();
        }

        private SystemOnDrop() { }
        
        void IEventListener.OnEvent(string eventKey, object eventData)
        {
            if (eventKey == UiEvents.UiDropEvent && eventData is JObject ed)
            {
                _uiEventData = ed;
            }
        }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(UiEvents.UiDropEvent, this);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            var world = systems.GetWorld();
            var viewPool = world.GetPool<ComponentDragDropView>();
            
            if (_uiEventData.TryGetValue("entity_id", out int entityId) 
                && viewPool.Has(entityId)
                && _uiEventData.TryGetVector("world_position", out Vector3 position))
            {
                viewPool.Get(entityId).View.OnDrop(position);
            }
            
            _uiEventData = null;
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(UiEvents.UiDropEvent, this);
        }
    }
}