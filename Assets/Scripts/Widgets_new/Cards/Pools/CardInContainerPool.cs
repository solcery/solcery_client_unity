using System.Collections.Generic;
using Solcery.Games;
using Solcery.Widgets_new.Cards.Widgets;
using UnityEngine;

namespace Solcery.Widgets_new.Cards.Pools
{
    public sealed class CardInContainerPool : ICardInContainerPool
    {
        private Transform _poolTransform;
        private Stack<ICardInContainerWidget> _pool;
        private IGame _game;

        public static ICardInContainerPool Create(Transform parent, IGame game)
        {
            return new CardInContainerPool(parent, game);
        }

        private CardInContainerPool(Transform parent, IGame game)
        {
            var go = new GameObject("card_in_container_pool")
            {
                transform =
                {
                    parent = parent
                }
            };
            go.transform.localScale = Vector3.one;
            go.SetActive(false);
            _poolTransform = go.GetComponent<Transform>();

            _game = game;

            _pool = new Stack<ICardInContainerWidget>();
        }

        bool ICardInContainerPool.TryPop(out ICardInContainerWidget cardInContainerWidget)
        {
            if (_pool.Count <= 0)
            {
                PrePool();
            }

            return _pool.TryPop(out cardInContainerWidget);
        }

        void ICardInContainerPool.Push(ICardInContainerWidget cardInContainerWidget)
        {
            cardInContainerWidget.Cleanup();
            cardInContainerWidget.UpdateParent(_poolTransform);
            _pool.Push(cardInContainerWidget);
        }

        void ICardInContainerPool.Destroy()
        {
            while (_pool.TryPop(out var cardInContainerWidget))
            {
                cardInContainerWidget.Destroy();
            }
            _pool = null;
            
            Object.Destroy(_poolTransform.gameObject);
            _poolTransform = null;
            
            _game = null;
        }

        private void PrePool()
        {
            if (_game.ServiceResource.TryGetWidgetPrefabForKey("ui/ui_card", out var prefab))
            {
                for (var index = 0; index < 10; index++)
                {
                    _pool.Push(CardInContainerWidget.Create(_game, prefab, _poolTransform));
                }
            }
        }
    }
}