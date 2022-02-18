using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;

namespace Solcery.Models.Play.Tooltip
{
    public interface ISystemOnTooltipHide : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemOnTooltipHide : ISystemOnTooltipHide
    {
        private JObject _uiEventData;
        
        public static ISystemOnTooltipHide Create()
        {
            return new SystemOnTooltipHide();
        }
        
        private SystemOnTooltipHide() { }     
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(UiEvents.UiTooltipHideEvent, this);
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(UiEvents.UiTooltipHideEvent, this);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            if (_uiEventData.TryGetValue("tooltip_id", out int tooltipId))
            {
                HideCurrentTooltip();
            }
            
            _uiEventData = null;
        }

        void IEventListener.OnEvent(string eventKey, object eventData)
        {
            if (eventKey == UiEvents.UiTooltipHideEvent && eventData is JObject ed)
            {
                _uiEventData = ed;
            }
        }

        private void HideCurrentTooltip()
        {
            GameApplication.Game().TooltipController.Hide();
        }
    }
}