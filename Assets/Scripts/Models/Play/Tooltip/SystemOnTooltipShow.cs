using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Tooltips;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Models.Play.Tooltip
{
    public interface ISystemOnTooltipShow : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemOnTooltipShow : ISystemOnTooltipShow
    {
        private JObject _uiEventData;
        private EcsFilter _tooltipsFilter;
        
        public static ISystemOnTooltipShow Create()
        {
            return new SystemOnTooltipShow();
        }
        
        private SystemOnTooltipShow() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(UiEvents.UiTooltipShowEvent, this);
            var world = systems.GetWorld();
            _tooltipsFilter = world.Filter<ComponentTooltips>().End();;
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(UiEvents.UiTooltipShowEvent, this);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            var world = systems.GetWorld();
            var tooltips = new Dictionary<int, JObject>();
            foreach (var objectTypesEntityId in _tooltipsFilter)
            {
                tooltips = world.GetPool<ComponentTooltips>().Get(objectTypesEntityId).Tooltips;
                break;
            }
            
            if (_uiEventData.TryGetValue("tooltip_id", out int tooltipId) 
                && _uiEventData.TryGetVector("world_position", out Vector2 position)
                && (tooltips.TryGetValue(tooltipId, out var tooltipDataObject)))
            {
                var text = tooltipDataObject.GetValue<string>("text");
                GameApplication.Game().TooltipController.Show(text, position);
            }
        }
        
        void IEventListener.OnEvent(string eventKey, object eventData)
        {
            if (eventKey == UiEvents.UiTooltipShowEvent && eventData is JObject ed)
            {
                _uiEventData = ed;
            }
        }
    }
}