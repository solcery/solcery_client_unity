using Newtonsoft.Json.Linq;
using Solcery.Widgets_new.Cards.Pools;

namespace Solcery.Widgets_new.EclipseToken
{
    public interface ITokenInContainerWidget : IPoolingWidget
    {
        void IncreaseCounter();
        void ClearCounter();
        void UpdateFromCardTypeData(int objectId, JObject data);
    }
}
