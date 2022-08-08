using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public class PlaceWidgetEclipseLayout : PlaceWidgetEclipseLayoutBase
    {
        [SerializeField]
        private RectTransform content;
        
        public override void SetAnchor(TextAnchor anchor)
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
