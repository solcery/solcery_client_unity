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

        private List<IPrefabLoader> _prefabLoaders;
        private Action<Dictionary<string, GameObject>> _callback;
        private int _completedLoaderCount;
        private Dictionary<string, GameObject> _widgetPrefabs;

        public static ILoadTask Create(Action<Dictionary<string, GameObject>> callback)
        {
            var widgetResourcePaths = new List<string>();
            
            var names = Enum.GetNames(typeof(PlaceWidgetTypes));
            foreach (var name in names)
            {
                if (Enum.TryParse(name, out PlaceWidgetTypes value) 
                    && EnumPlaceWidgetPrefabPathAttribute.TryGetPrefabPath(value, out var prefabPath)
                    && !string.IsNullOrEmpty(prefabPath))
                {
                    widgetResourcePaths.Add(prefabPath);
                }
            }

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
            //Debug.Log("TaskLoadWidgetPrefab ILoadTask.Run()");
            _completedLoaderCount = _prefabLoaders.Count;

            while (_prefabLoaders.Count > 0)
            {
                var prefabLoader = _prefabLoaders[0];
                _prefabLoaders.RemoveAt(0);
                prefabLoader.Load(OnPrefabLoaded);
            }
            
            //Debug.Log("TaskLoadWidgetPrefab ILoadTask.Run() Finish");
        }

        private void OnPrefabLoaded(IPrefabLoader obj)
        {
            //Debug.Log("TaskLoadWidgetPrefab OnPrefabLoaded");
            --_completedLoaderCount;

            if (!_widgetPrefabs.ContainsKey(obj.Name))
            {
                _widgetPrefabs.Add(obj.Name, obj.Prefab);
            }

            //Debug.Log("TaskLoadWidgetPrefab OnPrefabLoaded check completed");
            if (_completedLoaderCount <= 0)
            {
                _callback?.Invoke(_widgetPrefabs);
                Completed?.Invoke(true, this);
                //Debug.Log("TaskLoadWidgetPrefab OnPrefabLoaded completed");
            }
        }

        void ILoadTask.Destroy()
        {
            //Debug.Log("TaskLoadWidgetPrefab ILoadTask.Destroy()");
            
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