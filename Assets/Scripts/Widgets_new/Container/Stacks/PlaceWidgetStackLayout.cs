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
        private float offsetX;
        
        [SerializeField] [Range(0f, 1f)]
        private float offsetY;
        
        [SerializeField]
        private RectTransform content;
        
        [SerializeField]
        private RectTransform textArea;
        
        [SerializeField]
        private TMP_Text text;

        public int Capacity => capacity;

        public Vector3 GetCardOffset(int index)
        {
            index = index >= capacity - 1  ? capacity - 1 : index;
            return new Vector3(content.rect.width * offsetX * index, content.rect.height * offsetY * index, 0f);
        }
        
        public void UpdateText(string newText)
        {
            text.text = newText;
            textArea.localPosition = GetCardOffset(capacity);
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