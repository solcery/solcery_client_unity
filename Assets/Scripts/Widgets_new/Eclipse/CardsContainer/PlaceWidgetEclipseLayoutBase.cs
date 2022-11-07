using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public abstract class PlaceWidgetEclipseLayoutBase : PlaceWidgetLayout
    {
        [SerializeField]
        private GameObject waitDropFrame;

        public abstract void SetLayout(EventTrackerLayout layout, TextAnchor anchor);
        public abstract void AddCard(IEclipseCardInContainerWidget eclipseCardInContainerWidget);
        public abstract void Rebuild();
        
        public void Wait(bool isWait)
        {
            if (waitDropFrame != null)
            {
                waitDropFrame.SetActive(isWait);
            }
        }
    }
}
