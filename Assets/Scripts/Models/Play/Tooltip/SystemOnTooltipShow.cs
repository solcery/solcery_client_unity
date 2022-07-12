using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
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
        private readonly IGame _game;
        
        public static ISystemOnTooltipShow Create(IGame game)
        {
            return new SystemOnTooltipShow(game);
        }

        private SystemOnTooltipShow(IGame game)
        {
            _game = game;
        }
        
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
                GameApplication.Game().TooltipController.Show(_game, world, tooltipDataObject, onTooltipShowEventData.WorldPosition);
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