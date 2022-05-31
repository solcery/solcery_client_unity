using Solcery.Services.Events;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Cards.EventsData
{
    public class OnEclipseCardFullEventData : EventData
    {
        public const string OnEclipseCardFullEventName = "UI_ECLIPSE_CARD_FULL_EVENT";
        public readonly EclipseCardInContainerWidgetLayout EclipseCard;
        
        public static OnEclipseCardFullEventData Create(EclipseCardInContainerWidgetLayout eclipseCard)
        {
            return new OnEclipseCardFullEventData(eclipseCard);
        }
        
        private OnEclipseCardFullEventData(EclipseCardInContainerWidgetLayout eclipseCard) : base(OnEclipseCardFullEventName)
        {
            EclipseCard = eclipseCard;
        }
    }
}