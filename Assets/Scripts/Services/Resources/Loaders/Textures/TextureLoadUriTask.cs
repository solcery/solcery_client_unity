using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.Textures
{
    public sealed class TextureLoadUriTask : ITextureLoadUriTask
    {
        private List<ITextureLoaderUri> _textureLoaderUriList;
        private Dictionary<string, Texture2D> _textures;
        private Action<Dictionary<string, Texture2D>> _callback;

        public static ITextureLoadUriTask Create(List<string> imageUriList)
        {
            return new TextureLoadUriTask(imageUriList);
        }

        private TextureLoadUriTask(List<string> imageUriList)
        {
            _textureLoaderUriList = new List<ITextureLoaderUri>(imageUriList.Count);
            foreach (var imageUri in imageUriList)
            {
                _textureLoaderUriList.Add(TextureLoaderUri.Create(imageUri));
            }

            _textures = new Dictionary<string, Texture2D>(_textureLoaderUriList.Count);
        }

        void ITextureLoadUriTask.Run(Action<Dictionary<string, Texture2D>> callback)
        {
            _callback = callback;

            foreach (var imageLoaderUri in _textureLoaderUriList)
            {
                imageLoaderUri.Load(OnImageLoad);
            }
        }

        private void OnImageLoad(ITextureLoaderUri imageLoaderUri)
        {
            if (imageLoaderUri.LoadedIsSuccess)
            {
                _textures.Add(imageLoaderUri.Uri, imageLoaderUri.Texture);
            }
            
            _textureLoaderUriList.Remove(imageLoaderUri);
            imageLoaderUri.Destroy();

            if (_textureLoaderUriList.Count <= 0)
            {
                _callback?.Invoke(_textures);
            }
        }

        void ITextureLoadUriTask.Destroy()
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