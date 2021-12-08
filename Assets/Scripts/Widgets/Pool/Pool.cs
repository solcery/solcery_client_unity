using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Solcery.Widgets.Pool
{
    public class Pool : IDisposable
    {
        public string Name { get; }
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Stack<PoolObject> _cache;

        public Pool(GameObject prefab, Transform parent, int preLoadCount)
        {
            _prefab = prefab;
            _parent = parent;
            Name = _prefab.name;
            
            _cache = new Stack<PoolObject>();
            for (var i = 0; i < preLoadCount; i++)
            {
                _cache.Push(CreateObject());
            }
        }

        public PoolObject Pop()
        {
            while (_cache.Count > 0)
            {
                var clone = _cache.Pop();
                if (clone != null)
                    return clone;
            }

            return CreateObject();
        }

        public void Push(PoolObject poolable)
        {
            if (poolable != null)
            {
                _cache.Push(poolable);
                poolable.transform.SetParent(_parent, false);
            }
        }

        private PoolObject CreateObject()
        {
            var clone = Object.Instantiate(_prefab, _parent, false);
            var poolComponent = clone.GetComponent<PoolObject>();
            poolComponent.Create();
            if (poolComponent != null)
            {
                return poolComponent;
            }
            
            Debug.LogError($"Prefab {_prefab.name} doesn't contain \"PoolObject\" component!");
            return null;
        }

        public void Clear()
        {
            foreach (var poolObject in _cache)
            {
                Object.DestroyImmediate(poolObject.gameObject);
            }

            _cache.Clear();
        }

        public void Dispose()
        {
            _cache.Clear();
            if (_parent != null)
            {
                Object.DestroyImmediate(_parent.gameObject);
            }
        }        
    }
}