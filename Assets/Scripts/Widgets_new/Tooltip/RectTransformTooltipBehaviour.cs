using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformTooltipBehaviour : TooltipBehaviour
    {
        private RectTransform _rectTransform;
        
        public void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public override void OnPointerMove(PointerEventData eventData)
        {
            var position = eventData.position;
            if (RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, position))
            {
                ShowTooltip(position);
            }
        }
    }
}