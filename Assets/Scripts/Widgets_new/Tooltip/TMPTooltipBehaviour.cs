using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTooltipBehaviour : MonoBehaviour, IPointerExitHandler , IPointerMoveHandler
    {
        private TextMeshProUGUI _text;
        private int _linkIndex = -1;
        
        public void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);;
            if (linkIndex != -1)
            {
                _linkIndex = linkIndex;
                var tooltipId = _text.textInfo.linkInfo[_linkIndex].GetLinkID();
                GameApplication.Game().TooltipController.Show(tooltipId, eventData.position);
            }
            else
            {
                _linkIndex = -1;
                GameApplication.Game().TooltipController.Hide();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_linkIndex != -1)
            {
                GameApplication.Game().TooltipController.Hide();
            }
        }
    }
}
