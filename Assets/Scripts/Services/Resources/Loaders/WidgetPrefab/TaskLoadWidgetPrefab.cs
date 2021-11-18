using System;
using System.Collections.Generic;
using Solcery.Services.Resources.Patterns;
using Solcery.Services.Resources.Patterns.Widgets;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.WidgetPrefab
{
    public sealed class TaskLoadWidgetPrefab : ILoadTask
    {
        public event Action<bool, ILoadTask> Completed;

        private List<string> _widgetResourcePathList;
        private Action<Dictionary<string, GameObject>> _callback;

        public static ILoadTask Create(List<PatternData> patternDataList, Action<Dictionary<string, GameObject>> callback)
        {
            var widgetResourcePaths = new List<string>(patternDataList.Count);
            foreach (var patternRawData in patternDataList)
            {
                if (patternRawData is PatternWidgetData patternData)
                {
                    if (widgetResourcePaths.Contains(patternData.WidgetResourcePath))
                    {
                        continue;
                    }
                    
                    widgetResourcePaths.Add(patternData.WidgetResourcePath);
                }
            }
            
            return new TaskLoadWidgetPrefab(widgetResourcePaths, callback);
        }

        private TaskLoadWidgetPrefab(List<string> widgetResourcePaths, Action<Dictionary<string, GameObject>> callback)
        {
            _callback = callback;
            _widgetResourcePathList = new List<string>(widgetResourcePaths);
        }

        void ILoadTask.Run()
        {
            var widgetPrefabs = new Dictionary<string, GameObject>();
            
            foreach (var widgetResourcePath in _widgetResourcePathList)
            {
                widgetPrefabs.Add(widgetResourcePath, UnityEngine.Resources.Load<GameObject>(widgetResourcePath));
            }
            
            _callback?.Invoke(widgetPrefabs);
            Completed?.Invoke(true, this);
        }

        void ILoadTask.Destroy()
        {
            _callback = null;
            
            _widgetResourcePathList?.Clear();
            _widgetResourcePathList = null;
        }
    }
}