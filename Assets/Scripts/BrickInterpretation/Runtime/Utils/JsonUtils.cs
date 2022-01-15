using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Runtime.Utils
{
    internal static class JsonUtils
    {
        public static bool HasKey(this JObject token, string key)
        {
            return token.ContainsKey(key);
        }
        
        public static T GetValue<T>(this JObject token, string key)
        {
            if (token == null || !token.TryGetValue(key, StringComparison.Ordinal, out var value))
            {
                return default;
            }

            return value.Value<T>();
        }

        public static bool TryGetValue<T>(this JObject token, string key, out T ret)
        {
            if (token != null &&
                token.TryGetValue(key, StringComparison.Ordinal, out var value))
            {
                ret = value.Value<T>();
                return true;
            }

            ret = default;
            return false;
        }
        
        public static T GetEnum<T>(this JObject data, string key) where T : struct
        {
            if (data == null || !data.TryGetValue(key, StringComparison.Ordinal, out var value))
            {
                return default;
            }

            switch (value.Type)
            {
                case JTokenType.String:
                {
                    return Enum.TryParse(value.Value<string>(), out T ret) ? ret : default;   
                }

                case JTokenType.Integer:
                {
                    return (T)Enum.ToObject(typeof(T), value.Value<int>());
                }
                
                default:
                    return default;
            }
        }
        
        public static bool TryGetEnum<T>(this JObject data, string key, out T ret) where T : struct
        {
            if (data == null 
                || !data.ContainsKey(key) 
                || !data.TryGetValue(key, StringComparison.Ordinal, out var value))
            {
                ret = default;
                return false;
            }
            
            switch (value.Type)
            {
                case JTokenType.String:
                {
                    if (Enum.TryParse(value.Value<string>(), out T retL))
                    {
                        ret = retL;
                        return true;
                    }
                    
                    ret = default;
                    return false;
                }

                case JTokenType.Integer:
                {
                    ret = (T)Enum.ToObject(typeof(T), value.Value<int>());
                    return true;
                }
                
                default:
                    ret = default;
                    return false;
            }
        }

        public static bool LoadFromFile(string path, out JToken result)
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                result = null;
                return false;
            }
            
            using (var jsonReader = new JsonTextReader(new StreamReader(fullPath)))
            {
                result = JToken.Load(jsonReader);
                return true;
            }
        }

        public static bool SaveForFile<T>(this T data, string path) where T : JToken
        {
            var fullPath = Path.GetFullPath(path);
            using (var outputFile = new StreamWriter(fullPath))
            {
                outputFile.WriteLine(data.ToString());
                return true;
            }
        }
    }
}