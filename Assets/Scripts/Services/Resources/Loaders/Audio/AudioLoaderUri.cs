using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Solcery.Services.Resources.Loaders.Audio
{
    public sealed class AudioLoaderUri : IAudioLoaderUri
    {
        bool IAudioLoaderUri.LoadedIsSuccess => _loadedIsSuccess;
        int IAudioLoaderUri.SoundId => _soundId;
        AudioClip IAudioLoaderUri.Clip => _clip;
        
        private bool _loadedIsSuccess;
        private int _soundId;
        private AudioClip _clip;
        private bool _inProgress;
        private Action<IAudioLoaderUri> _callback;
        private UnityWebRequest _webRequest;

        public static IAudioLoaderUri Create(int soundId, string uri)
        {
            return new AudioLoaderUri(soundId, uri);
        }

        private AudioLoaderUri(int soundId, string uri)
        {
            _loadedIsSuccess = false;
            _soundId = soundId;
            _clip = null;
            _inProgress = false;
            _webRequest = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV);
            _webRequest.timeout = 30;
        }

        void IAudioLoaderUri.Load(Action<IAudioLoaderUri> callback)
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
                _clip = ((DownloadHandlerAudioClip) _webRequest.downloadHandler).audioClip;
                Debug.Log($"Load clip {_soundId} type {_clip.loadType}");
            }
            
            _webRequest.Dispose();

            _inProgress = false;
            _callback?.Invoke(this);
        }

        void IAudioLoaderUri.Destroy()
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