using Newtonsoft.Json.Linq;

namespace Solcery.Accessors.Cache
{
    public interface ICacheAccessor
    {
        public JObject GetMetadata();
        public JObject GetCacheForKey(string key);
        public void UpdateMetadataForKey(string key, JObject dataJObject);
    }
}
