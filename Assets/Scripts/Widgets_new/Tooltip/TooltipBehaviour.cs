using Solcery.Services.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    public abstract class TooltipBehaviour : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler, IPointerUpHandler
    {
        protected int TooltipId = -1;
        protected bool Active;
        
        public abstract void OnPointerMove(PointerEventData eventData);
        
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (Active)
            {
                HideTooltip();
            }
        }
        
        public void SetTooltipId(int tooltipId)
        {
            Active = false;
            TooltipId = tooltipId;
        }

        protected void ShowTooltip(Vector2 position)
        {
            if (TooltipId != -1)
            {
                Active = true;
                ServiceEvents.Current.BroadcastEvent(OnTooltipShowEventData.Create(TooltipId, position));
            }
        }

        protected virtual void HideTooltip()
        {
            if (!Active)
                return;
            
            Active = false;
            ServiceEvents.Current.BroadcastEvent(OnTooltipHideEventData.Create(TooltipId));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Active)
            {
                HideTooltip();
            }
        }
    }
}