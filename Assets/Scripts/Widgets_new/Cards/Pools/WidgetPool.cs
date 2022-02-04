using System;
using System.Collections.Generic;
using Solcery.Games;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Solcery.Widgets_new.Cards.Pools
{
    public sealed class WidgetPool<T> : IWidgetPool<T> where T : IPoolingWidget
    {
        private Transform _poolTransform;
        private Stack<T> _pool;
        private IGame _game;
        
        private readonly string _prefabKey;
        private readonly Func<IGame, GameObject, Transform, T> _factoryFunc;

        public static WidgetPool<T> Create(Transform parent, IGame game, string prefabKey, Func<IGame, GameObject, Transform, T> factoryFunc)
        {
            return new WidgetPool<T>(parent, game, prefabKey, factoryFunc);
        }
        
        private WidgetPool(Transform parent, IGame game, string prefabKey, Func<IGame, GameObject, Transform, T> factoryFunc)
        {
            var go = new GameObject("card_in_container_pool")
            {
                transform =
                {
                    parent = parent,
                    localScale = Vector3.one
                }
            };
            go.SetActive(false);
            
            _poolTransform = go.GetComponent<Transform>();
            _game = game;
            _pool = new Stack<T>();
            _prefabKey = prefabKey;
            _factoryFunc = factoryFunc;
        }

        bool IWidgetPool<T>.TryPop(out T poolingWidget)
        {
            if (_pool.Count <= 0)
            {
                PrePool();
            }

            return _pool.TryPop(out poolingWidget);
        }

        void IWidgetPool<T>.Push(T poolingWidget)
        {
            poolingWidget.Cleanup();
            poolingWidget.UpdateParent(_poolTransform);
            _pool.Push(poolingWidget);
        }

        void IWidgetPool<T>.Destroy()
        {
            while (_pool.TryPop(out var poolingWidget))
            {
                poolingWidget.Destroy();
            }
            _pool = null;
            
            Object.Destroy(_poolTransform.gameObject);
            _poolTransform = null;
            _game = null;
        }
        
        private void PrePool()
        {
            if (!_game.ServiceResource.TryGetWidgetPrefabForKey(_prefabKey, out var prefab))
            {
                return;
            }
            
            for (var index = 0; index < 10; index++)
            {
                _pool.Push(_factoryFunc.Invoke(_game, prefab, _poolTransform));
            }
        }
    }
}