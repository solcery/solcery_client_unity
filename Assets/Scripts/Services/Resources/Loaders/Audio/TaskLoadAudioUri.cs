using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.Audio
{
    public sealed class TaskLoadAudioUri : ILoadTask
    {
        public event Action<bool, ILoadTask> Completed;
        public event Action<float> Progress;
        
        private int _completedLoaderCount;
        private int _allLoaderCount;
        private List<IAudioLoaderUri> _audioLoaderUriList;
        private Dictionary<int, AudioClip> _audioClips;
        private Action<Dictionary<int, AudioClip>> _callback;
        
        public static ILoadTask Create(List<Tuple<int, string>> audioUriList, Action<Dictionary<int, AudioClip>> callback)
        {
            return new TaskLoadAudioUri(audioUriList, callback);
        }

        private TaskLoadAudioUri(List<Tuple<int, string>> audioUriList, Action<Dictionary<int, AudioClip>> callback)
        {
            _callback = callback;
            _audioLoaderUriList = new List<IAudioLoaderUri>(audioUriList.Count);
            var hashAddUri = new HashSet<int>();
            foreach (var audioUri in audioUriList)
            {
                if (!hashAddUri.Contains(audioUri.Item1))
                {
                    hashAddUri.Add(audioUri.Item1);
                    _audioLoaderUriList.Add(AudioLoaderUri.Create(audioUri.Item1, audioUri.Item2));
                }
            }

            _audioClips = new Dictionary<int, AudioClip>(_audioLoaderUriList.Count);
        }

        void ILoadTask.Run()
        {
            if (_audioLoaderUriList.Count <= 0)
            {
                Progress?.Invoke(1f);
                _callback?.Invoke(_audioClips);
                Completed?.Invoke(true, this);
                return;
            }
            
            _completedLoaderCount = _audioLoaderUriList.Count;
            _allLoaderCount = _completedLoaderCount > 0 ? _completedLoaderCount : 1;

            while (_audioLoaderUriList.Count > 0)
            {
                var audioLoaderUri = _audioLoaderUriList[0];
                _audioLoaderUriList.RemoveAt(0);
                audioLoaderUri.Load(OnAudioLoad);
            }
        }
        
        private void OnAudioLoad(IAudioLoaderUri audioLoaderUri)
        {
            --_completedLoaderCount;
            if (audioLoaderUri.LoadedIsSuccess)
            {
                _audioClips.Add(audioLoaderUri.SoundId, audioLoaderUri.Clip);
            }
            
            audioLoaderUri.Destroy();
            
            if (_completedLoaderCount <= 0)
            {
                Progress?.Invoke(1f);
                _callback?.Invoke(_audioClips);
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

            if (_audioLoaderUriList != null)
            {
                foreach (var audioLoaderUri in _audioLoaderUriList)
                {
                    audioLoaderUri.Destroy();
                }
            }

            _audioLoaderUriList?.Clear();
            _audioLoaderUriList = null;
            
            _audioClips?.Clear();
            _audioClips = null;
        }
    }
}