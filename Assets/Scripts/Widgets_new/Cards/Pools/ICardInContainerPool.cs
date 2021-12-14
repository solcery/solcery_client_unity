using Solcery.Widgets_new.Cards.Widgets;

namespace Solcery.Widgets_new.Cards.Pools
{
    public interface ICardInContainerPool
    {
        bool TryPop(out ICardInContainerWidget cardInContainerWidget);
        void Push(ICardInContainerWidget cardInContainerWidget);
        void Destroy();
    }
}