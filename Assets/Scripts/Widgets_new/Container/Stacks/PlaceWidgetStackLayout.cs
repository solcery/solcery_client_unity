using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.CardsContainer;
using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStackLayout : PlaceWidgetEclipseLayoutBase
    {
        [SerializeField]
        private RectTransform content;
        
        [SerializeField]
        private TMP_Text text;

        public void UpdateText(string newText)
        {
            text.text = newText;
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
            eclipseCardInContainerWidget.UpdateSiblingIndex(0);
            eclipseCardInContainerWidget.Layout.RectTransform.anchoredPosition = Vector3.zero;
        }

        public override void Rebuild()
        {
        }
    }
}