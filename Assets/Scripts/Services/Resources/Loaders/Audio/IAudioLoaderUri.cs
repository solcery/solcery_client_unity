using System;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.Audio
{
    public interface IAudioLoaderUri
    {
        bool LoadedIsSuccess { get; }
        int SoundId { get; }
        AudioClip Clip { get; }

        void Load(Action<IAudioLoaderUri> callback);
        void Destroy();
    }
}