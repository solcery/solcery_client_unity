using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Solcery.Services.Resources.Loaders.WidgetPrefab
{
    public sealed class PrefabLoader : IPrefabLoader
    {
        string IPrefabLoader.Name => _widgetPrefabPath;
        GameObject IPrefabLoader.Prefab => _widgetPrefab;

        private Action<IPrefabLoader> _callback;
        private string _widgetPrefabPath;
        private GameObject _widgetPrefab;

        public static IPrefabLoader Create(string widgetPrefabPath)
        {
            return new PrefabLoader(widgetPrefabPath);
        }
        
        private PrefabLoader(string widgetPrefabPath)
        {
            _widgetPrefabPath = widgetPrefabPath;
        }

        void IPrefabLoader.Load(Action<IPrefabLoader> callback)
        {
            _callback = callback;
            Addressables.LoadAssetAsync<GameObject>(_widgetPrefabPath).Completed += OnCompleted;
        }

        void IPrefabLoader.Destroy()
        {
            _callback = null;
            _widgetPrefabPath = null;
        }

        private void OnCompleted(AsyncOperationHandle<GameObject> obj)
        {
            obj.Completed -= OnCompleted;
            _widgetPrefab = obj.Result;
            _callback?.Invoke(this);
        }
    }
}