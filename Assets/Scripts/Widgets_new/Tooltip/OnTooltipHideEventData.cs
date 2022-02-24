using Solcery.Services.Events;

namespace Solcery.Widgets_new.Tooltip
{
    public class OnTooltipHideEventData : EventData
    {
        public const string OnTooltipEventName = "UI_TOOLTIP_HIDE_EVENT";
        
        public int TooltipId => _tooltipId;
        
        private readonly int _tooltipId;
        
        public static OnTooltipHideEventData Create(int tooltipId)
        {
            return new OnTooltipHideEventData(tooltipId);
        }
        
        private OnTooltipHideEventData(int tooltipId) : base(OnTooltipEventName)
        {
            _tooltipId = tooltipId;
        }
    }
}