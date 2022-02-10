using Newtonsoft.Json.Linq;
using Solcery.Widgets_new.Cards.Pools;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public interface IEclipseCardInContainerWidget : IPoolingWidget
    {
        EclipseCardInContainerWidgetLayout Layout { get; }

        void UpdateFromCardTypeData(int objectId, JObject data);
        void SetEclipseCardType(EclipseCardInContainerWidgetTypes eclipseCardType);
        void UpdateSiblingIndex(int siblingIndex);
    }
}