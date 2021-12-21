using System;
using System.Collections.Generic;
using Solcery.Services.Resources.Patterns;
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

        public static ILoadTask Create(List<PatternData> patternDataList, Action<Dictionary<string, GameObject>> callback)
        {
            var widgetResourcePaths = new List<string>
            {
                "ui/ui_widget",
                "ui/ui_title",
                "ui/ui_button",
                "ui/ui_picture",
                "ui/ui_hand",
                "ui/ui_stack",
                "ui/ui_card"
            };

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
            _completedLoaderCount = 0;
            foreach (var prefabLoader in _prefabLoaders)
            {
                prefabLoader.Load(OnPrefabLoaded);
            }
        }

        private void OnPrefabLoaded(IPrefabLoader obj)
        {
            ++_completedLoaderCount;
            
            _widgetPrefabs.Add(obj.Name, obj.Prefab);

            if (_prefabLoaders.Count <= _completedLoaderCount)
            {
                _callback?.Invoke(_widgetPrefabs);
                Completed?.Invoke(true, this);
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