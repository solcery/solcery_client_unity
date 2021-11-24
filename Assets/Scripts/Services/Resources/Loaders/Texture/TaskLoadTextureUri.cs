using System;
using System.Collections.Generic;
using Solcery.Services.Resources.Patterns;
using Solcery.Services.Resources.Patterns.Texture;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.Texture
{
    public sealed class TaskLoadTextureUri : ILoadTask
    {
        public event Action<bool, ILoadTask> Completed;
        
        private int _completedLoaderCount;
        private List<ITextureLoaderUri> _textureLoaderUriList;
        private Dictionary<string, Texture2D> _textures;
        private Action<Dictionary<string, Texture2D>> _callback;
        
        
        public static ILoadTask Create(List<PatternData> patternDataList, Action<Dictionary<string, Texture2D>> callback)
        {
            var imageUriList = new List<string>(patternDataList.Count);
            foreach (var patternRawData in patternDataList)
            {
                if (patternRawData is PatternUriTextureData patternData)
                {
                    imageUriList.Add(patternData.Uri);
                }
            }

            return new TaskLoadTextureUri(imageUriList, callback);
        }

        public static ILoadTask Create(List<string> imageUriList, Action<Dictionary<string, Texture2D>> callback)
        {
            return new TaskLoadTextureUri(imageUriList, callback);
        }

        private TaskLoadTextureUri(List<string> imageUriList, Action<Dictionary<string, Texture2D>> callback)
        {
            _callback = callback;
            _textureLoaderUriList = new List<ITextureLoaderUri>(imageUriList.Count);
            foreach (var imageUri in imageUriList)
            {
                _textureLoaderUriList.Add(TextureLoaderUri.Create(imageUri));
            }

            _textures = new Dictionary<string, Texture2D>(_textureLoaderUriList.Count);
        }

        void ILoadTask.Run()
        {
            _completedLoaderCount = 0;
            foreach (var imageLoaderUri in _textureLoaderUriList)
            {
                imageLoaderUri.Load(OnImageLoad);
            }
        }

        private void OnImageLoad(ITextureLoaderUri imageLoaderUri)
        {
            ++_completedLoaderCount;
            if (imageLoaderUri.LoadedIsSuccess)
            {
                _textures.Add(imageLoaderUri.Uri, imageLoaderUri.Texture);
            }
            
            imageLoaderUri.Destroy();

            if (_textureLoaderUriList.Count <= _completedLoaderCount)
            {
                _callback?.Invoke(_textures);
                Completed?.Invoke(true, this);
            }
        }

        void ILoadTask.Destroy()
        {
            _callback = null;

            if (_textureLoaderUriList != null)
            {
                foreach (var imageLoaderUri in _textureLoaderUriList)
                {
                    imageLoaderUri.Destroy();
                }
            }

            _textureLoaderUriList?.Clear();
            _textureLoaderUriList = null;
            
            _textures?.Clear();
            _textures = null;
        }
    }
}