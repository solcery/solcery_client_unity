using Newtonsoft.Json.Linq;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public interface ITokenInContainerWidget : IPoolingWidget, IDraggableWidget, IEntityId
    {
        void IncreaseCounter();
        void ClearCounter();
        void UpdateFromCardTypeData(int objectId, JObject data);
    }
}
