using Solcery.Services.Events;

namespace Solcery.Widgets_new.Eclipse.Cards.EventsData
{
    public class OnLeftClickEventData : EventData
    {
        public const string OnLeftClickEventName = "UI_LEFT_CLICK_EVENT";
        
        public int EntityId { get; }

        public static OnLeftClickEventData Create(int entityId)
        {
            return new OnLeftClickEventData(entityId);
        }

        private OnLeftClickEventData(int entityId) : base(OnLeftClickEventName)
        {
            EntityId = entityId;
        }        
    }
}