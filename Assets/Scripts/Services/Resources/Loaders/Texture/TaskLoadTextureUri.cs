using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.Texture
{
    public sealed class TaskLoadTextureUri : ILoadTask
    {
        public event Action<bool, ILoadTask> Completed;
        public event Action<float> Progress;
        
        private int _completedLoaderCount;
        private int _allLoaderCount;
        private List<ITextureLoaderUri> _textureLoaderUriList;
        private Dictionary<string, Texture2D> _textures;
        private Action<Dictionary<string, Texture2D>> _callback;

        public static ILoadTask Create(List<string> imageUriList, Action<Dictionary<string, Texture2D>> callback)
        {
            return new TaskLoadTextureUri(imageUriList, callback);
        }

        private TaskLoadTextureUri(List<string> imageUriList, Action<Dictionary<string, Texture2D>> callback)
        {
            _callback = callback;
            _textureLoaderUriList = new List<ITextureLoaderUri>(imageUriList.Count);
            var hashAddUri = new HashSet<string>();
            foreach (var imageUri in imageUriList)
            {
                if (!hashAddUri.Contains(imageUri))
                {
                    hashAddUri.Add(imageUri);
                    _textureLoaderUriList.Add(TextureLoaderUri.Create(imageUri));
                }
            }

            _textures = new Dictionary<string, Texture2D>(_textureLoaderUriList.Count);
        }

        void ILoadTask.Run()
        {
            if (_textureLoaderUriList.Count <= 0)
            {
                Progress?.Invoke(1f);
                _callback?.Invoke(_textures);
                Completed?.Invoke(true, this);
                return;
            }
            
            _completedLoaderCount = _textureLoaderUriList.Count;
            _allLoaderCount = _completedLoaderCount > 0 ? _completedLoaderCount : 1;

            while (_textureLoaderUriList.Count > 0)
            {
                var imageLoaderUri = _textureLoaderUriList[0];
                _textureLoaderUriList.RemoveAt(0);
                imageLoaderUri.Load(OnImageLoad);
            }
        }

        private void OnImageLoad(ITextureLoaderUri imageLoaderUri)
        {
            --_completedLoaderCount;
            if (imageLoaderUri.LoadedIsSuccess)
            {
                _textures.Add(imageLoaderUri.Uri, imageLoaderUri.Texture);
            }
            
            imageLoaderUri.Destroy();
            
            if (_completedLoaderCount <= 0)
            {
                Progress?.Invoke(1f);
                _callback?.Invoke(_textures);
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