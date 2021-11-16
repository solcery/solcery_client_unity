using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Services.Resources
{
    public interface IServiceResource
    {
        void PreloadResourcesFromGameContent(JObject gameContentJson);
        public bool GetTextureByKey(string key, out Texture2D texture);
        void Cleanup();
        void Destroy();
    }
}