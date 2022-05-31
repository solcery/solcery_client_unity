using Solcery.Widgets_new.Cards.Pools;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public interface IListTokensInContainerWidget : IPoolingWidget
    {
        ListTokensInContainerWidgetLayout Layout { get; }
        void AddToken(ITokenInContainerWidget eclipseToken);
        void DecreaseCounter();
        void IncreaseCounter();
        void ClearCounter();
    }
}
