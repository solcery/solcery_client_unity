using Solcery.Games;
using Solcery.Widgets_new.Cards.Pools;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public class ListTokensInContainerWidget : IListTokensInContainerWidget
    {
        private IGame _game;
        private ListTokensInContainerWidgetLayout _layout;
        private int _counter;
        
        public ListTokensInContainerWidgetLayout Layout => _layout;
        
        public static ListTokensInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new ListTokensInContainerWidget(game, prefab, poolTransform);
        }
        
        private ListTokensInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<ListTokensInContainerWidgetLayout>();
            ClearCounter();
        }

        public void AddToken(ITokenInContainerWidget eclipseToken)
        {
            eclipseToken.UpdateParent(Layout.Content);
            eclipseToken.Layout.transform.localPosition = Vector3.zero;
        }

        public void IncreaseCounter()
        {
            _layout.UpdateCounter(++_counter);
        }

        public void DecreaseCounter()
        {
            _layout.UpdateCounter(--_counter);
        }
        
        public void ClearCounter()
        {
            _layout.UpdateCounter(_counter = 0);
        }

        void IPoolingWidget.UpdateParent(Transform parent)
        {
            _layout.UpdateParent(parent);
        }

        void IPoolingWidget.Cleanup()
        {
        }

        void IPoolingWidget.Destroy()
        {
            Object.Destroy(_layout.gameObject);
            _layout = null;
            _game = null;
        }
        
        void IPoolingWidget.BackToPool()
        {
            _game.ListTokensInContainerWidgetPool.Push(this);
        }
    }
}
