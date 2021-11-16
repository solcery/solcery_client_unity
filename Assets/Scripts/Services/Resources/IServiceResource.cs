using Newtonsoft.Json.Linq;

namespace Solcery.Services.Resources
{
    public interface IServiceResource
    {
        void PreloadResourcesFromGameContent(JObject gameContentJson);
        void Cleanup();
        void Destroy();
    }
}