namespace Solcery.Services.Events
{
    public interface IServiceEvents
    {
        IServiceEvents AddListener(string eventKey, IEventListener eventListener);
        IServiceEvents RemoveListener(string eventKey, IEventListener eventListener);
        void BroadcastEvent(string eventKey, object eventData);
        void Cleanup();
    }
}