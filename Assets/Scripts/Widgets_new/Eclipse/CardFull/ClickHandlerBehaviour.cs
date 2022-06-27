using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Solcery
{
    public class ClickHandlerBehaviour : MonoBehaviour, IPointerClickHandler
    {
        public UnityAction OnLeftClick { get; set; }
        public UnityAction OnRightClick { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    OnLeftClick?.Invoke();
                    break;
                case PointerEventData.InputButton.Right:
                    OnRightClick?.Invoke();
                    break;
            }
        }

        public void Cleanup()
        {
            OnLeftClick = null;
            OnRightClick = null;
        }
    }
}
