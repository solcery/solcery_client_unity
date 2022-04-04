using Newtonsoft.Json.Linq;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public interface IEclipseCardInContainerWidget : IPoolingWidget, IDraggableWidget, IEntityId
    {
        EclipseCardInContainerWidgetLayout Layout { get; }
        
        void UpdateFromCardTypeData(int objectId, JObject data);
        void SetEclipseCardType(EclipseCardInContainerWidgetTypes eclipseCardType);
        void UpdateSiblingIndex(int siblingIndex);
        void UpdateTokenSlots(int count);
        void AttachToken(int slot, JObject data);
        void UpdateTimer(bool show, int duration);
    }
}