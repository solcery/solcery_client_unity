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
            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                _rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );
            
            ShowTooltip(position);
        }
    }
}