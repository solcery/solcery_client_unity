using UnityEngine;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public sealed class CardInContainerWidget : ICardInContainerWidget
    {
        private CardInContainerWidgetLayout _layout;

        public static ICardInContainerWidget Create(GameObject prefab, Transform poolTransform)
        {
            return new CardInContainerWidget(prefab, poolTransform);
        }
        
        private CardInContainerWidget(GameObject prefab, Transform poolTransform)
        {
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<CardInContainerWidgetLayout>();
        }
        
        void ICardInContainerWidget.Cleanup()
        {
            _layout.Cleanup();
        }

        void ICardInContainerWidget.Destroy()
        {
            Object.Destroy(_layout.gameObject);
            _layout = null;
        }
    }
}