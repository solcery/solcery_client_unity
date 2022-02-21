namespace Solcery.Services.Events
{
    public abstract class EventData
    {
        public string EventName => _eventName;

        private string _eventName;

        protected EventData(string eventName)
        {
            _eventName = eventName;
        }
    }
}