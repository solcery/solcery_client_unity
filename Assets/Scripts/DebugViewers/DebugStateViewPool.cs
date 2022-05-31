using System.Collections.Generic;
using UnityEngine;

namespace Solcery.DebugViewers
{
    public sealed class DebugStateViewPool<T> where T : MonoBehaviour
    {
        private RectTransform _parent;
        private GameObject _prefab;
        private Stack<T> _debugStateViewPool;

        public static DebugStateViewPool<T> Create(RectTransform parent, GameObject prefab, int capacity)
        {
            return new DebugStateViewPool<T>(parent, prefab, capacity);
        }
        
        private DebugStateViewPool(RectTransform parent, GameObject prefab, int capacity)
        {
            _parent = parent;
            _prefab = prefab;
            _debugStateViewPool = new Stack<T>(capacity);
            AddDebugStateViewToPool(capacity);
        }

        private void AddDebugStateViewToPool(int capacity)
        {
            for (var i = 0; i < capacity; i++)
            {
                var debugStateView = Object.Instantiate(_prefab, _parent).GetComponent<T>();
                _debugStateViewPool.Push(debugStateView);
            }
        }

        public void Push(T debugStateView)
        {
            Transform transform;
            (transform = debugStateView.transform).SetParent(_parent);
            transform.localPosition = Vector3.zero;
            _debugStateViewPool.Push(debugStateView);
        }

        public T Pop()
        {
            if (_debugStateViewPool.Count <= 0)
            {
                AddDebugStateViewToPool(10);
            }

            return _debugStateViewPool.Pop();
        }

        public void Cleanup()
        {
            while (_debugStateViewPool.Count > 0)
            {
                var view = _debugStateViewPool.Pop();
                Object.Destroy(view.gameObject);
            }
        }
    }
}