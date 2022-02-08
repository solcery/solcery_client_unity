using Solcery.Widgets_new.Cards.Pools;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public interface IEclipseCardInContainerWidget : IPoolingWidget
    {
        void SetEclipseCardType(EclipseCardInContainerWidgetTypes eclipseCardType);
    }
}