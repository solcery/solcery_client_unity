namespace Solcery.Services.Events
{
    public interface IEventListener
    {
        void OnEvent(string eventKey, object eventData);
    }
}