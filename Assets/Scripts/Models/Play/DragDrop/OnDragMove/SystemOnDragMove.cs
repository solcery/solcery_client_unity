using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Play.DragDrop.OnDragMove
{
    public interface ISystemOnDragMove : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemOnDragMove : ISystemOnDragMove
    {
        private JObject _uiEventData;
        
        public static ISystemOnDragMove Create()
        {
            return new SystemOnDragMove();
        }

        private SystemOnDragMove() { }
        
        void IEventListener.OnEvent(string eventKey, object eventData)
        {
            if (eventKey == UiEvents.UiDragMoveEvent && eventData is JObject ed)
            {
                _uiEventData = ed;
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
            
            if (_uiEventData.TryGetValue("entity_id", out int entityId) 
                && viewPool.Has(entityId)
                && _uiEventData.TryGetVector("world_position", out Vector3 position))
            {
                viewPool.Get(entityId).View.OnMove(position);
            }
            
            _uiEventData = null;
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(UiEvents.UiDragMoveEvent, this);
        }
    }
}