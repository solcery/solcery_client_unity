using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Solcery.Services.Resources.Loaders.Texture
{
    public sealed class TextureLoaderUri : ITextureLoaderUri
    {
        bool ITextureLoaderUri.LoadedIsSuccess => _loadedIsSuccess;
        string ITextureLoaderUri.Uri => _uri;
        Texture2D ITextureLoaderUri.Texture => _texture;

        private bool _loadedIsSuccess;
        private readonly string _uri;
        private Texture2D _texture;
        private bool _inProgress;
        private Action<ITextureLoaderUri> _callback;
        private UnityWebRequest _webRequest;

        public static TextureLoaderUri Create(string uri)
        {
            return new TextureLoaderUri(uri);
        }
        
        private TextureLoaderUri(string uri)
        {
            _loadedIsSuccess = false;
            _uri = uri;
            _texture = null;
            _inProgress = false;
            _webRequest = UnityWebRequestTexture.GetTexture(_uri);
            _webRequest.timeout = 30;
        }

        void ITextureLoaderUri.Load(Action<ITextureLoaderUri> callback)
        {
            _callback = callback;
            _inProgress = true;
            _webRequest.SendWebRequest().completed += OnCompleted;
        }

        private void OnCompleted(AsyncOperation obj)
        {
            obj.completed -= OnCompleted;

            if (!_inProgress)
            {
                return;
            }
            
            if (_webRequest.result == UnityWebRequest.Result.Success)
            {
                _loadedIsSuccess = true;
                _texture = ((DownloadHandlerTexture) _webRequest.downloadHandler).texture;
                _texture.Compress(true);
                //Debug.Log($"Load texture from uri {_uri} WxH {_texture.width}x{_texture.height} format {_texture.format}");
            }
            
            _webRequest.Dispose();

            _inProgress = false;
            _callback?.Invoke(this);
        }

        void ITextureLoaderUri.Destroy()
        {
            if (_inProgress)
            {
                _inProgress = false;
                _webRequest.Abort();
            }
            
            _webRequest.Dispose();
        }
    }
}