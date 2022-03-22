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
                if (patternRawData is PatternUriTextureData patternData 
                    && !imageUriList.Contains(patternData.Uri))
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
            //Debug.Log("TaskLoadTextureUri ILoadTask.Run()");
            _completedLoaderCount = _textureLoaderUriList.Count;

            while (_textureLoaderUriList.Count > 0)
            {
                var imageLoaderUri = _textureLoaderUriList[0];
                _textureLoaderUriList.RemoveAt(0);
                imageLoaderUri.Load(OnImageLoad);
            }
            
            //Debug.Log("TaskLoadTextureUri ILoadTask.Run() finish");
        }

        private void OnImageLoad(ITextureLoaderUri imageLoaderUri)
        {
            //Debug.Log("TaskLoadTextureUri OnPrefabLoaded");
            --_completedLoaderCount;
            if (imageLoaderUri.LoadedIsSuccess)
            {
                _textures.Add(imageLoaderUri.Uri, imageLoaderUri.Texture);
            }
            
            imageLoaderUri.Destroy();
            

            //Debug.Log("TaskLoadTextureUri OnPrefabLoaded check completed");
            if (_completedLoaderCount <= 0)
            {
                _callback?.Invoke(_textures);
                Completed?.Invoke(true, this);
                
                //Debug.Log("TaskLoadTextureUri OnPrefabLoaded completed");
            }
        }

        void ILoadTask.Destroy()
        {
            //Debug.Log("TaskLoadTextureUri ILoadTask.Destroy()");
            
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