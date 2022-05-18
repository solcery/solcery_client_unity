using Solcery.Services.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    public abstract class TooltipBehaviour : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler
    {
        public int TooltipId;
        public bool Active = false;
        
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
            TooltipId = tooltipId;
            Active = true;
        }

        protected void ShowTooltip(Vector2 position)
        {
            if (Active)
            {
                ServiceEvents.Current.BroadcastEvent(OnTooltipShowEventData.Create(TooltipId, position));
            }
            else
            {
                Debug.Log("TooltipId doesn't set!");
            }
        }

        protected virtual void HideTooltip()
        {
            if (!Active)
                return;

            ServiceEvents.Current.BroadcastEvent(OnTooltipHideEventData.Create(TooltipId));
        }        
    }
}