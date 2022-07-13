namespace Solcery.Widgets_new.Timer
{
    public interface IPlaceWidgetTimer
    {
        void Start(int durationMsec);
        void Update(int durationMsec);
        void Stop();
    }
}