using Newtonsoft.Json.Linq;
using Solcery.Services.Events;
using Solcery.Ui;
using Solcery.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solcery.Widgets_new.Tooltip
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTooltipBehaviour : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler
    {
        private TextMeshProUGUI _text;
        private string _tooltipId;
        
        public void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);;
            if (linkIndex != -1)
            {
                ShowTooltip(_text.textInfo.linkInfo[linkIndex].GetLinkID(), eventData.position);
            }
            else
            {
                HideTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_tooltipId != null)
            {
                HideTooltip();
            }
        }

        private void ShowTooltip(string tooltipId, Vector2 position)
        {
            _tooltipId = tooltipId;
            var eventData = new JObject
            {
                {"tooltip_id", new JValue(tooltipId)},
                {"world_position", position.ToJObject()}
            };
            
            ServiceEvents.Current.BroadcastEvent(UiEvents.UiTooltipShowEvent, eventData);
        }

        private void HideTooltip()
        {
            if (_tooltipId == null)
                return;

            var eventData = new JObject
            {
                {"tooltip_id", new JValue(_tooltipId)},
            };
            ServiceEvents.Current.BroadcastEvent(UiEvents.UiTooltipHideEvent, eventData);
            _tooltipId = null;
        }
    }
}
