using System.Collections.Generic;
using Solcery.Services.Resources;
using Solcery.Services.Sound.Play;
using UnityEngine.Pool;

namespace Solcery.Services.Sound
{
    public sealed class ServiceSound : IServiceSound
    {
        private readonly Stack<SoundPlayController> _stack;
        private readonly IServiceResource _resource;

        private ObjectPool<SoundPlayController> _pool;
        private List<SoundPlayController> _playings;

        public static IServiceSound Create(SoundsLayout layout, IServiceResource resource)
        {
            return new ServiceSound(layout, resource);
        }

        private ServiceSound(SoundsLayout layout, IServiceResource resource)
        {
            _resource = resource;
            _pool = new ObjectPool<SoundPlayController>(layout.CreateSoundPlayController, null, ActionOnRelease);
            _playings = new List<SoundPlayController>();
        }

        private static void ActionOnRelease(SoundPlayController obj)
        {
            obj.Cleanup();
        }

        void IServiceSound.Play(int soundId)
        {
            if (!_resource.TryGetSoundForId(soundId, out var clip))
            {
                return;
            }
            
            var soundController = _pool.Get();
            _playings.Add(soundController);
            soundController.OnPlayFinished = OnPlayFinished;
            soundController.Play(clip);
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
    }
}