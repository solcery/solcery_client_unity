using System.Collections.Generic;
using Solcery.Services.Resources;
using Solcery.Widgets_new.Cards.Widgets;
using UnityEngine;

namespace Solcery.Widgets_new.Cards.Pools
{
    public sealed class CardInContainerPool : ICardInContainerPool
    {
        private Transform _poolTransform;
        private Stack<ICardInContainerWidget> _pool;
        private IServiceResource _serviceResource;

        public static ICardInContainerPool Create(Transform parent, IServiceResource serviceResource)
        {
            return new CardInContainerPool(parent, serviceResource);
        }

        private CardInContainerPool(Transform parent, IServiceResource serviceResource)
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

            _serviceResource = serviceResource;

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
            
            _serviceResource = null;
        }

        private void PrePool()
        {
            if (_serviceResource.TryGetWidgetPrefabForKey("ui/ui_card", out var prefab))
            {
                for (var index = 0; index < 10; index++)
                {
                    _pool.Push(CardInContainerWidget.Create(prefab, _poolTransform));
                }
            }
        }
    }
}