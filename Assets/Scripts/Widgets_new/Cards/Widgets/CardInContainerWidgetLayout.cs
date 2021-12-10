using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public sealed class CardInContainerWidgetLayout : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private GameObject back;
        [SerializeField]
        private TMP_Text cardName;
        [SerializeField]
        private TMP_Text description;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image image;

        public void UpdateParent(Transform parent)
        {
            rectTransform.SetParent(parent, false);
        }

        public void UpdatePosition(Vector3 position)
        {
            rectTransform.position = position;
        }
        
        public void Cleanup()
        {
            
        }
    }
}