using Solcery.Services.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    public abstract class TooltipBehaviour : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler
    {
        protected int? TooltipId;
        
        public abstract void OnPointerMove(PointerEventData eventData);
        
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (TooltipId != null)
            {
                HideTooltip();
            }
        }
        
        public void SetTooltipId(int tooltipId)
        {
            TooltipId = tooltipId;
        }

        protected void ShowTooltip(Vector2 position)
        {
            if (TooltipId != null)
            {
                ServiceEvents.Current.BroadcastEvent(OnTooltipShowEventData.Create(TooltipId.Value, position));
            }
            else
            {
                Debug.Log("TooltipId doesn't set!");
            }
        }

        protected virtual void HideTooltip()
        {
            if (TooltipId == null)
                return;

            ServiceEvents.Current.BroadcastEvent(OnTooltipHideEventData.Create(TooltipId.Value));
        }        
    }
}