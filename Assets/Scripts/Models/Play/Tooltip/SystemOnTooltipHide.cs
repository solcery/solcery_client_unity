using Leopotam.EcsLite;
using Solcery.Services.Events;
using Solcery.Widgets_new.Tooltip;

namespace Solcery.Models.Play.Tooltip
{
    public interface ISystemOnTooltipHide : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemOnTooltipHide : ISystemOnTooltipHide
    {
        private EventData _uiEventData;
        
        public static ISystemOnTooltipHide Create()
        {
            return new SystemOnTooltipHide();
        }
        
        private SystemOnTooltipHide() { }     
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnTooltipHideEventData.OnTooltipEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnTooltipHideEventData.OnTooltipEventName, this);
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            if (_uiEventData is OnTooltipHideEventData)
            {
                HideCurrentTooltip();
            }
            
            _uiEventData = null;
        }

        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnTooltipHideEventData.OnTooltipEventName)
            {
                _uiEventData = eventData;
            }
        }

        private void HideCurrentTooltip()
        {
            GameApplication.Game().TooltipController.Hide();
        }
    }
}