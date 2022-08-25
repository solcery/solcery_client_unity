using Newtonsoft.Json.Linq;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Services.Cache
{
    public class CacheAccessor : ICacheAccessor
    {
        public readonly string CacheFileDirectory = Application.persistentDataPath + "/Cached/";
        private const string ContentMetadataKey = "content_metadata";
        private const string MetadataKey = "metadata";
        private const string MetadataFileName = ContentMetadataKey + ".json";
        private readonly JToken _metadataJToken;
        
        public CacheAccessor()
        {
            if (!JsonUtils.LoadFromFile(CacheFileDirectory + MetadataFileName, out _metadataJToken))
            {
                _metadataJToken = new JObject {{ContentMetadataKey, new JObject()}};
            }
        }

        public JObject GetMetadata()
        {
            return _metadataJToken as JObject;
        }

        public JObject GetCacheForKey(string key)
        {
            JsonUtils.LoadFromFile(CacheFileDirectory + key + ".json", out var cacheToken);
            return cacheToken as JObject;
        }

        public void UpdateMetadataForKey(string key, JObject dataJObject)
        {
            var dataJToken = dataJObject as JToken;
            var metadataRootJToken = _metadataJToken[ContentMetadataKey];
            if (dataJToken[MetadataKey] != null && metadataRootJToken != null)
            {
                metadataRootJToken[key] = dataJToken[MetadataKey];
                _metadataJToken.SaveForFile(CacheFileDirectory + MetadataFileName);
                dataJObject.SaveForFile(CacheFileDirectory + key + ".json");
                Debug.Log($"Metadata for \"{key}\" was saved in cache!");
            }
            else
            {
                Debug.LogWarning($"Can't save metadata for \"{key}\" in cache!");
            }
        }

        public void ProcessCache(string key, ref JObject dataJObject)
        {
            if (dataJObject == null)
            {
                dataJObject = GetCacheForKey(key);
            }
            else
            {
                UpdateMetadataForKey(key, dataJObject);
            }
        }
    }
}
