using Solcery.Services.Events;

namespace Solcery.Widgets_new.Eclipse.Cards.EventsData
{
    public class OnRightClickEventData : EventData
    {
        public const string OnRightClickEventName = "UI_RIGHT_CLICK_EVENT";
        
        public int EntityId { get; }

        public static OnRightClickEventData Create(int entityId)
        {
            return new OnRightClickEventData(entityId);
        }

        private OnRightClickEventData(int entityId) : base(OnRightClickEventName)
        {
            EntityId = entityId;
        }
    }
}