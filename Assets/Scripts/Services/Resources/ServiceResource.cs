using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Services.Resources.Loaders.Textures;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Services.Resources
{
    public sealed class ServiceResource : IServiceResource
    {
        private IGameResourcesCallback _gameResourcesCallback;
        private ITextureLoadUriTask _task;
        private readonly Dictionary<string, Texture2D> _textures;

        public static IServiceResource Create(IGameResourcesCallback gameResourcesCallback)
        {
            return new ServiceResource(gameResourcesCallback);
        }

        private ServiceResource(IGameResourcesCallback gameResourcesCallback)
        {
            _textures = new Dictionary<string, Texture2D>();
            _gameResourcesCallback = gameResourcesCallback;
        }

        void IServiceResource.PreloadResourcesFromGameContent(JObject gameContentJson)
        {
            var imageUriList = new List<string>();
            if (gameContentJson.TryGetValue("cardTypes", out JObject ct) 
                && ct.TryGetValue("objects", out JArray entityTypeArray))
            {
                foreach (var entityTypeToken in entityTypeArray)
                {
                    if (entityTypeToken is JObject entityTypeObject)
                    {
                        var pictureUri = entityTypeObject.GetValue<string>("picture");
                        if (pictureUri != null)
                        {
                            imageUriList.Add(pictureUri);
                        }
                    }
                }
            }

            _task = TextureLoadUriTask.Create(imageUriList);
            _task.Run(OnImagesLoaded);
        }

        private void OnImagesLoaded(Dictionary<string, Texture2D> obj)
        {
            foreach (var kv in obj)
            {
                _textures.Add(kv.Key, kv.Value);
            }
            
            _task?.Destroy();
            _gameResourcesCallback?.OnResourcesLoad();
        }

        public bool GetTextureByKey(string key, out Texture2D texture)
        {
            return _textures.TryGetValue(key, out texture);
        }

        void IServiceResource.Cleanup()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _task?.Destroy();
            _task = null;
        }

        void IServiceResource.Destroy()
        {
            Cleanup();
            _gameResourcesCallback = null;
        }
    }
}