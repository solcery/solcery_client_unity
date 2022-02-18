using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    public abstract class TooltipBehaviour : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler
    {
        protected string TooltipId;
        
        public abstract void OnPointerMove(PointerEventData eventData);
        
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (TooltipId != null)
            {
                HideTooltip();
            }
        }
        
        public void SetTooltipId(string tooltipId)
        {
            TooltipId = tooltipId;
        }

        protected void ShowTooltip(Vector2 position)
        {
            if (TooltipId != null)
            {
                var eventData = new JObject
                {
                    {"tooltip_id", new JValue(TooltipId)},
                    {"world_position", position.ToJObject()}
                };

                ServiceEvents.Current.BroadcastEvent(UiEvents.UiTooltipShowEvent, eventData);
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

            var eventData = new JObject
            {
                {"tooltip_id", new JValue(TooltipId)},
            };
            ServiceEvents.Current.BroadcastEvent(UiEvents.UiTooltipHideEvent, eventData);
        }        
    }
}