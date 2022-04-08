using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Attributes.Values;
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
        void AttachToken(int index, JObject data);
        void UpdateFromAttributes(Dictionary<string, IAttributeValue> attributes);
    }
}