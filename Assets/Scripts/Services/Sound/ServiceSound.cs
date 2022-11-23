using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.React;
using Solcery.Services.Resources;
using Solcery.Services.Sound.Play;
using Solcery.Utils;
using UnityEngine.Pool;

namespace Solcery.Services.Sound
{
    public sealed class ServiceSound : IServiceSound
    {
        private readonly Stack<SoundPlayController> _stack;
        private readonly IServiceResource _resource;

        private ObjectPool<SoundPlayController> _pool;
        private List<SoundPlayController> _playings;
        private float _masterVolume;

        public static IServiceSound Create(SoundsLayout layout, IServiceResource resource)
        {
            return new ServiceSound(layout, resource);
        }

        private ServiceSound(SoundsLayout layout, IServiceResource resource)
        {
            _resource = resource;
            _pool = new ObjectPool<SoundPlayController>(layout.CreateSoundPlayController, null, ActionOnRelease);
            _playings = new List<SoundPlayController>();

            _masterVolume = 1f;
            ReactToUnity.AddCallback(ReactToUnity.EventOnSetMasterVolume, OnSetMasterVolume);
        }

        private void OnSetMasterVolume(string obj)
        {
            var objData = JObject.Parse(obj);
            _masterVolume = objData.GetValue<int>("volume");
        }

        private static void ActionOnRelease(SoundPlayController obj)
        {
            obj.Cleanup();
        }

        void IServiceSound.Play(int soundId, int volume)
        {
            if (!_resource.TryGetSoundForId(soundId, out var clip))
            {
                return;
            }
            
            var soundController = _pool.Get();
            _playings.Add(soundController);
            soundController.OnPlayFinished = OnPlayFinished;
            var resultVolume = volume / 100f;
            resultVolume *= _masterVolume;
            soundController.Play(clip, resultVolume);
        }

        private void OnPlayFinished(SoundPlayController obj)
        {
            obj.OnPlayFinished = null;
            _playings.Remove(obj);
            _pool.Release(obj);
        }

        void IServiceSound.Cleanup()
        {
            while (_playings.Count > 0)
            {
                var playController = _playings[0];
                playController.Cleanup();
            }
        }

        public void Destroy()
        {
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnSetMasterVolume, OnSetMasterVolume);
        }
    }
}