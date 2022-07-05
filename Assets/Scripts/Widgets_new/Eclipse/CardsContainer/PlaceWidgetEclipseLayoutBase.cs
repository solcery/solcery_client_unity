using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public abstract class PlaceWidgetEclipseLayoutBase : PlaceWidgetLayout
    {
        public abstract void SetAnchor(TextAnchor anchor);
        public abstract void AddCard(IEclipseCardInContainerWidget eclipseCardInContainerWidget);
        public abstract void Rebuild();
    }
}
