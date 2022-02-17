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
                ShowTooltip(_text.textInfo.linkInfo[_linkIndex].GetLinkID(), eventData.position);
            }
            else
            {
                HideTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_linkIndex != -1)
            {
                HideTooltip();
            }
        }

        private void ShowTooltip(string tooltipId, Vector2 position)
        {
            var ed = new JObject
            {
                {"tooltip_id", new JValue(tooltipId)},
                {"world_position", position.ToJObject()}
            };
            
            ServiceEvents.Current.BroadcastEvent(UiEvents.UiTooltipShowEvent, ed);
        }

        private void HideTooltip()
        {
            _linkIndex = -1;
            GameApplication.Game().TooltipController.Hide();
        }
    }
}
