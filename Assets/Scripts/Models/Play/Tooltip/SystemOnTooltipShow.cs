using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Tooltips;
using Solcery.Services.Events;
using Solcery.Utils;
using Solcery.Widgets_new.Tooltip;

namespace Solcery.Models.Play.Tooltip
{
    public interface ISystemOnTooltipShow : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemOnTooltipShow : ISystemOnTooltipShow
    {
        private EventData _uiEventData;
        private EcsFilter _tooltipsFilter;
        
        public static ISystemOnTooltipShow Create()
        {
            return new SystemOnTooltipShow();
        }
        
        private SystemOnTooltipShow() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnTooltipShowEventData.OnTooltipShowEventName, this);
            var world = systems.GetWorld();
            _tooltipsFilter = world.Filter<ComponentTooltips>().End();;
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnTooltipShowEventData.OnTooltipShowEventName, this);
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
            
            
            if (_uiEventData is OnTooltipShowEventData onTooltipShowEventData
                && tooltips.TryGetValue(onTooltipShowEventData.TooltipId, out var tooltipDataObject))
            {
                GameApplication.Game().TooltipController.Show(tooltipDataObject, onTooltipShowEventData.WorldPosition);
            }
            
            _uiEventData = null;
        }
        
        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnTooltipShowEventData.OnTooltipShowEventName)
            {
                _uiEventData = eventData;
            }
        }
    }
}