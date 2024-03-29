using System;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.Texture
{
    public interface ITextureLoaderUri
    {
        bool LoadedIsSuccess { get; }
        string Uri { get; }
        Texture2D Texture { get; }

        void Load(Action<ITextureLoaderUri> callback);
        void Destroy();
    }
}