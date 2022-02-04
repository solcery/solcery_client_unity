namespace Solcery.Widgets_new.Cards.Pools
{
    public interface IWidgetPool<T> where T : IPoolingWidget
    {
        bool TryPop(out T poolingWidget);
        void Push(T poolingWidget);
        void Destroy();
    }
}