using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public interface IEclipseCardInContainerWidget : IPoolingWidget, IDraggableWidget, IEntityId
    {
        int Order { get; }
        int EntityId { get; }
        int CardType { get; }
        EclipseCardInContainerWidgetLayout Layout { get; }
        void UpdateFromCardTypeData(int entityId, int objectId, int objectType, EclipseCardTypes type, IItemType itemType);
        void UpdateSiblingIndex(int siblingIndex);
        EclipseCardTokenLayout AttachToken(int slot, int objectId, IItemType itemType);
        Vector3 GetTokenPosition(int slot);
        void SetOrder(int order);
    }
}