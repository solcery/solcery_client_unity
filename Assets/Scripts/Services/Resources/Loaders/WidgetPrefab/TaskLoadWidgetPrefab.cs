using System;
using System.Collections.Generic;
using Solcery.Widgets_new;
using Solcery.Widgets_new.Attributes.Enum;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.WidgetPrefab
{
    public sealed class TaskLoadWidgetPrefab : ILoadTask
    {
        public event Action<bool, ILoadTask> Completed;
        public event Action<float> Progress;

        private List<IPrefabLoader> _prefabLoaders;
        private Action<Dictionary<string, GameObject>> _callback;
        private int _completedLoaderCount;
        private int _allLoaderCount;
        private Dictionary<string, GameObject> _widgetPrefabs;

        public static ILoadTask Create(List<string> widgetResourcePaths, Action<Dictionary<string, GameObject>> callback)
        {
            return new TaskLoadWidgetPrefab(widgetResourcePaths, callback);
        }

        private TaskLoadWidgetPrefab(List<string> widgetResourcePaths, Action<Dictionary<string, GameObject>> callback)
        {
            _callback = callback;
            _prefabLoaders = new List<IPrefabLoader>();
            _completedLoaderCount = 0;
            _widgetPrefabs = new Dictionary<string, GameObject>(widgetResourcePaths.Count);
            
            foreach (var widgetResourcePath in widgetResourcePaths)
            {
                _prefabLoaders.Add(PrefabLoader.Create(widgetResourcePath));
            }
        }

        void ILoadTask.Run()
        {
            if (_prefabLoaders.Count <= 0)
            {
                Progress?.Invoke(1f);
                _callback?.Invoke(_widgetPrefabs);
                Completed?.Invoke(true, this);
                return;
            }
            
            _completedLoaderCount = _prefabLoaders.Count;
            _allLoaderCount = _completedLoaderCount > 0 ? _completedLoaderCount : 1;

            while (_prefabLoaders.Count > 0)
            {
                var prefabLoader = _prefabLoaders[0];
                _prefabLoaders.RemoveAt(0);
                prefabLoader.Load(OnPrefabLoaded);
            }
        }

        private void OnPrefabLoaded(IPrefabLoader obj)
        {
            --_completedLoaderCount;

            if (!_widgetPrefabs.ContainsKey(obj.Name))
            {
                _widgetPrefabs.Add(obj.Name, obj.Prefab);
            }

            if (_completedLoaderCount <= 0)
            {
                Progress?.Invoke(1f);
                _callback?.Invoke(_widgetPrefabs);
                Completed?.Invoke(true, this);
            }
            else
            {
                Progress?.Invoke(1f - _completedLoaderCount / (float)_allLoaderCount);
            }
        }

        void ILoadTask.Destroy()
        {
            _callback = null;
            
            _widgetPrefabs?.Clear();
            _widgetPrefabs = null;

            if (_prefabLoaders != null)
            {
                foreach (var prefabLoader in _prefabLoaders)
                {
                    prefabLoader.Destroy();
                }
                
                _prefabLoaders.Clear();
            }

            _prefabLoaders = null;
        }
    }
}