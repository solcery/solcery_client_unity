using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.React;
using Solcery.Services.GameContent;
using Solcery.Services.Resources.Loaders;
using Solcery.Services.Resources.Loaders.Audio;
using Solcery.Services.Resources.Loaders.Multi;
using Solcery.Services.Resources.Loaders.Texture;
using Solcery.Services.Resources.Loaders.WidgetPrefab;
using Solcery.Widgets_new;
using Solcery.Widgets_new.Attributes.Enum;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Solcery.Services.Resources
{
    public sealed class ServiceResource : IServiceResource
    {
        private IGameResourcesCallback _gameResourcesCallback;
        private IMultiLoadTask _task;
        private readonly Dictionary<string, Texture2D> _textures;
        private readonly Dictionary<int, AudioClip> _clips;
        private readonly Dictionary<string, GameObject> _prefabs;

        private int _lastProgress;

        public static IServiceResource Create(IGameResourcesCallback gameResourcesCallback)
        {
            return new ServiceResource(gameResourcesCallback);
        }

        private ServiceResource(IGameResourcesCallback gameResourcesCallback)
        {
            _textures = new Dictionary<string, Texture2D>();
            _clips = new Dictionary<int, AudioClip>();
            _prefabs = new Dictionary<string, GameObject>();
            _gameResourcesCallback = gameResourcesCallback;
        }

        void IServiceResource.PreloadResourcesFromGameContent(IServiceGameContent serviceGameContent)
        {
            _lastProgress = -1;
            UpdateLoadingProgress(0f);
            // Prepare widget list
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
            
            _task = MultiLoadTask.Create();
            _task.Completed += OnCompletedAllTask;
            _task.Progress += UpdateLoadingProgress;
            _task.AddTask(TaskLoadTextureUri.Create(serviceGameContent.ItemTypes.PictureUriList, OnImagesLoaded));
            _task.AddTask(TaskLoadAudioUri.Create(serviceGameContent.Sounds, OnAudioLoaded));
            _task.AddTask(TaskLoadWidgetPrefab.Create(widgetResourcePaths, OnWidgetPrefabLoaded));
            _task.Run();
        }

        private void OnCompletedAllTask(bool result, ILoadTask task)
        {
            if (_task == task)
            {
                _task.Completed -= OnCompletedAllTask;
                _task.Progress -= UpdateLoadingProgress;
                _task.Destroy();
                _task = null;
            }

            UpdateLoadingProgress(1f);
            _gameResourcesCallback?.OnResourcesLoad();
        }

        private void OnImagesLoaded(Dictionary<string, Texture2D> obj)
        {
            foreach (var kv in obj)
            {
                _textures.Add(kv.Key, kv.Value);
            }
        }
        
        private void OnAudioLoaded(Dictionary<int, AudioClip> obj)
        {
            foreach (var kv in obj)
            {
                _clips.Add(kv.Key, kv.Value);
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
        
        bool IServiceResource.TryGetSoundForId(int id, out AudioClip clip)
        {
            return _clips.TryGetValue(id, out clip);
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
            _clips.Clear();
            _textures.Clear();
        }

        void IServiceResource.Destroy()
        {
            Cleanup();
            _gameResourcesCallback = null;
        }

        private void UpdateLoadingProgress(float progress)
        {
            var iProgress = (int)(progress * 100f);
            
            if (_lastProgress >= iProgress)
            {
                return;
            }

            _lastProgress = iProgress;
            
            var jProgress = new JObject
            {
                { "progress", new JValue(iProgress) },
                { "state", new JObject() }
            };

            UnityToReact.Instance.CallOnUnityLoadProgress(jProgress.ToString(Formatting.None));
        }
    }
}