using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public class PlaceWidgetEclipseLayout : PlaceWidgetLayout
    {
        [SerializeField]
        private ScrollRect scroll;

        public void AddCard(IEclipseCardInContainerWidget eclipseCardInContainerWidget)
        {
            eclipseCardInContainerWidget.UpdateParent(scroll.content);
            eclipseCardInContainerWidget.UpdateSiblingIndex(0);
            scroll.horizontal = scroll.content.childCount > 1;
        }
    }
}