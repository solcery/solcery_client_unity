using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Services.Resources
{
    public interface IServiceResource
    {
        void PreloadResourcesFromGameContent(JObject gameContentJson);
        bool TryGetTextureForKey(string key, out Texture2D texture);
        bool TryGetWidgetPrefabForKey(string key, out GameObject prefab);
        void Cleanup();
        void Destroy();
    }
}