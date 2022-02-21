using Solcery.Services.Events;
using UnityEngine;

namespace Solcery.Widgets_new.Tooltip
{
    public class OnTooltipShowEventData : EventData
    {
        public const string OnTooltipShowEventName = "UI_TOOLTIP_SHOW_EVENT";
        
        public int TooltipId => _tooltipId;
        public Vector3 WorldPosition => _worldPosition;
        
        private readonly int _tooltipId;
        private readonly Vector3 _worldPosition;
        
        public static OnTooltipShowEventData Create(int tooltipId, Vector3 worldPosition)
        {
            return new OnTooltipShowEventData(tooltipId, worldPosition);
        }
        
        private OnTooltipShowEventData(int tooltipId, Vector3 worldPosition) : base(OnTooltipShowEventName)
        {
            _tooltipId = tooltipId;
            _worldPosition = worldPosition;
        }
    }
}