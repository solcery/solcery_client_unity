using Solcery.Services.GameContent;
using UnityEngine;

namespace Solcery.Services.Resources
{
    public interface IServiceResource
    {
        void PreloadResourcesFromGameContent(IServiceGameContent serviceGameContent);
        bool TryGetTextureForKey(string key, out Texture2D texture);
        bool TryGetWidgetPrefabForKey(string key, out GameObject prefab);
        void Cleanup();
        void Destroy();
    }
}