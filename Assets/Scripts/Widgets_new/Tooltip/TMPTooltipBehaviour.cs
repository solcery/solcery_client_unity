using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTooltipBehaviour : TooltipBehaviour
    {
        private TextMeshProUGUI _text;
        
        public void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public override void OnPointerMove(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);;
            if (linkIndex != -1)
            {
                SetTooltipId(_text.textInfo.linkInfo[linkIndex].GetLinkID());
                ShowTooltip(eventData.position);
            }
            else
            {
                HideTooltip();
            }
        }

        protected override void HideTooltip()
        {
            base.HideTooltip();
            TooltipId = null;
        }
    }
}
