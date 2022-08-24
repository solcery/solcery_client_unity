using Newtonsoft.Json.Linq;

namespace Solcery.Services.Cache
{
    public interface ICacheService
    {
        public JObject GetMetadata();
        public JObject GetCacheForKey(string key);
        public void UpdateMetadataForKey(string key, JObject dataJObject);
        public void ProcessCache(string key, ref JObject dataJObject);
    }
}
