using System.Collections.Generic;
using Leopotam.EcsLite;
using ModestTree;
using UnityEngine;

namespace Solcery.Services.Widget
{
    public class PoolProvider
    {
        private Transform _root;
        private readonly Dictionary<GameObject, Pool> _pools;
        private readonly Dictionary<PoolObject, Pool> _given;
        private EcsWorld _world;
        
        public PoolProvider(string poolName)
        {
            _pools = new Dictionary<GameObject, Pool>();
            _given = new Dictionary<PoolObject, Pool>();

            var go = new GameObject(poolName);
            Object.DontDestroyOnLoad(go);
            _root = go.transform;
        }

        public void Init(EcsWorld world)
        {
            _world = world;
        }

        public void CreatePool(GameObject prefab, int preloadCount = 0)
        {
            if (_pools.ContainsKey(prefab))
            {
                return;
            }
            
            _pools.Add(prefab, new Pool(prefab, _root.transform, preloadCount, _world));
        }

        public T GetFromPool<T>(GameObject prefab, bool active = true) where T : PoolObject
        {
            return GetFromPool<T>(prefab, null, active);
        }

        public T GetFromPool<T>(GameObject prefab, PoolObject parent, bool active = true) where T : PoolObject
        {
            if (!_pools.ContainsKey(prefab))
            {
                CreatePool(prefab, 1);
            }

            var result = _pools[prefab].Pop();
            _given[result] = _pools[prefab];
            if (result.gameObject.activeSelf != active)
            {
                result.gameObject.SetActive(active);
            }

            if (parent != null)
            {
                result.transform.SetParent(parent.transform, false);
            }
            else
            {
                result.transform.SetParent(null);
            }
            result.Init(_world);
            return result as T;
        }
        
        public void ReturnToPool(PoolObject poolObject)
        {
            if (poolObject.gameObject == null)
            {
                Log.Error("Object to push to the pool is null!");
                return;
            }

            if (_given.TryGetValue(poolObject, out var pool))
            {
                poolObject.Clear();
                pool.Push(poolObject);
                _given.Remove(poolObject);
            }
            else
            {
                Object.Destroy(poolObject.gameObject);
                Log.Error("No pool for " + poolObject.name + " available. Object will be destroyed!");
            }
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            CheckGiven();
#endif
        }

        public void ReturnAllToPool()
        {
            foreach (var pool in _given)
            {
                pool.Key.Clear();
                pool.Value.Push(pool.Key);
            }
            _given.Clear();
        }

        public void Clear()
        {
            if (_root == null)
                return;

            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }

            _pools.Clear();
            _given.Clear();
        }

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
                pool.Dispose();
            }

            _pools.Clear();
            _given.Clear();

            if (_root != null && _root.gameObject != null)
            {
                Object.DestroyImmediate(_root.gameObject);
                _root = null;
            }
        }

        private void CheckGiven()
        {
            foreach (var give in _given)
            {
                if (give.Key == null)
                {
                    Log.Error($"Something wrong with {give.Value.Name} in pool!");
                }
            }
        }        
    }
}