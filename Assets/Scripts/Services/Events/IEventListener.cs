namespace Solcery.Services.Events
{
    public interface IEventListener
    {
        void OnEvent(EventData eventData);
    }
}