using Leopotam.EcsLite;

namespace Solcery.Widgets
{
    public interface IWidget
    {
        WidgetViewBase View { get; }
        void UpdateSubWidgets(EcsWorld world, int[] entityIds);
        WidgetViewBase CreateView();
        void ClearView();

    }
}