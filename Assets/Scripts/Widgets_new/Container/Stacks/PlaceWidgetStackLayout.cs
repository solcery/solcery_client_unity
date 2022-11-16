using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.CardsContainer;
using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStackLayout : PlaceWidgetEclipseLayoutBase
    {
        [SerializeField]
        private int capacity = 1;
        
        [SerializeField] [Range(0f, 1f)]
        private float offset;
        
        [SerializeField]
        private RectTransform content;
        
        [SerializeField]
        private RectTransform textArea;
        
        [SerializeField]
        private TMP_Text text;

        public int Capacity => capacity;

        public float GetCardOffset(int index)
        {
            index = index >= capacity - 1  ? capacity - 1 : index;
            return content.rect.width * offset * index;
        }

        public void UpdateText(string newText)
        {
            text.text = newText;
            textArea.localPosition = new Vector3 (GetCardOffset(capacity), 0, 0);
        }

        public void UpdateTextVisible(bool visible)
        {
            text.gameObject.SetActive(visible);
        }
        
        public override void SetLayout(EventTrackerLayout layout, TextAnchor anchor)
        {
        }
        
        public override void AddCard(IEclipseCardInContainerWidget eclipseCardInContainerWidget)
        {
            eclipseCardInContainerWidget.UpdateParent(content);
            eclipseCardInContainerWidget.Layout.RectTransform.anchoredPosition = Vector3.zero;
        }

        public override void Rebuild()
        {
        }
    }
}