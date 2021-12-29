using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Services.Resources.Loaders;
using Solcery.Services.Resources.Loaders.Multi;
using Solcery.Services.Resources.Loaders.Texture;
using Solcery.Services.Resources.Loaders.WidgetPrefab;
using Solcery.Services.Resources.Patterns;
using Solcery.Services.Resources.Patterns.Texture;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Solcery.Services.Resources
{
    public sealed class ServiceResource : IServiceResource
    {
        private IGameResourcesCallback _gameResourcesCallback;
        private IMultiLoadTask _task;
        private readonly Dictionary<string, Texture2D> _textures;
        private readonly Dictionary<string, GameObject> _prefabs;

        public static IServiceResource Create(IGameResourcesCallback gameResourcesCallback)
        {
            return new ServiceResource(gameResourcesCallback);
        }

        private ServiceResource(IGameResourcesCallback gameResourcesCallback)
        {
            _textures = new Dictionary<string, Texture2D>();
            _prefabs = new Dictionary<string, GameObject>();
            _gameResourcesCallback = gameResourcesCallback;
        }

        void IServiceResource.PreloadResourcesFromGameContent(JObject gameContentJson)
        {
            //Debug.Log("PreloadResourcesFromGameContent start");
            var patternsProcessor = PatternsProcessor.Create();
            patternsProcessor.PatternRegistration(PatternUriTexture.Create());
            patternsProcessor.ProcessGameContent(gameContentJson);

            _task = MultiLoadTask.Create();
            _task.Completed += OnCompletedAllTask;

            if (patternsProcessor.TryGetAllPatternDataForType(PatternTypes.UriTexture, out var imageUriList))
            {
                _task.AddTask(TaskLoadTextureUri.Create(imageUriList, OnImagesLoaded));
            }

            _task.AddTask(TaskLoadWidgetPrefab.Create(OnWidgetPrefabLoaded));

            //Debug.Log("PreloadResourcesFromGameContent _task.Run()");
            _task.Run();
        }

        private void OnCompletedAllTask(bool result, ILoadTask task)
        {
            if (_task == task)
            {
                _task.Completed -= OnCompletedAllTask;
                _task.Destroy();
                _task = null;
            }

            _gameResourcesCallback?.OnResourcesLoad();
        }

        private void OnImagesLoaded(Dictionary<string, Texture2D> obj)
        {
            foreach (var kv in obj)
            {
                _textures.Add(kv.Key, kv.Value);
            }
        }
        
        private void OnWidgetPrefabLoaded(Dictionary<string, GameObject> obj)
        {
            foreach (var kv in obj)
            {
                _prefabs.Add(kv.Key, kv.Value);
            }
        }

        bool IServiceResource.TryGetTextureForKey(string key, out Texture2D texture)
        {
            return _textures.TryGetValue(key, out texture);
        }

        bool IServiceResource.TryGetWidgetPrefabForKey(string key, out GameObject prefab)
        {
            return _prefabs.TryGetValue(key, out prefab);
        }

        void IServiceResource.Cleanup()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _task?.Destroy();
            _task = null;

            foreach (var prefab in _prefabs)
            {
                Addressables.Release(prefab.Value);
            }
            _prefabs.Clear();
            
            _textures.Clear();
        }

        void IServiceResource.Destroy()
        {
            Cleanup();
            _gameResourcesCallback = null;
        }
    }
}