using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.Textures
{
    public interface ITextureLoadUriTask
    {
        void Run(Action<Dictionary<string, Texture2D>> callback);
        void Destroy();
    }
}