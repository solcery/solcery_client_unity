using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformTooltipBehaviour : TooltipBehaviour
    {
        public override void OnPointerMove(PointerEventData eventData)
        {
            ShowTooltip(eventData.position);
        }
    }
}