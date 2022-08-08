using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;

namespace Solcery.Widgets_new.Eclipse.Nft.Card
{
    public interface IEclipseCardNftInContainerWidget : IPoolingWidget, IDraggableWidget, IEntityId
    {
        EclipseCardNftInContainerWidgetLayout Layout { get; }
        int Order { get; }
        int EntityId { get; }
        int CardType { get; }
        void UpdateFromCardTypeData(int entityId, int objectId, int objectType, EclipseCardTypes type, IItemType itemType);
        void UpdateSiblingIndex(int siblingIndex);
        void SetOrder(int order);
    }
}