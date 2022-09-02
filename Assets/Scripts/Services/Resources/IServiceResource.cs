using Solcery.Services.GameContent;
using UnityEngine;

namespace Solcery.Services.Resources
{
    public interface IServiceResource
    {
        void PreloadResourcesFromGameContent(IServiceGameContent serviceGameContent);
        bool TryGetTextureForKey(string key, out Texture2D texture);
        bool TryGetSoundForId(int id, out AudioClip clip);
        bool TryGetWidgetPrefabForKey(string key, out GameObject prefab);
        void Cleanup();
        void Destroy();
    }
}