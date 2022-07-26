using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Utils
{
    public static class JsonUtils
    {
        public static bool HasKey(this JObject token, string key)
        {
            return token.ContainsKey(key);
        }

        public static T GetValue<T>(this JToken value)
        {
            return value.Value<T>();
        }
        
        public static T GetValue<T>(this JObject token, string key)
        {
            if (token == null || !token.TryGetValue(key, StringComparison.Ordinal, out var value))
            {
                return default;
            }

            return value.GetValue<T>();
        }

        public static bool TryGetValue<T>(this JObject token, string key, out T ret)
        {
            if (token != null &&
                token.TryGetValue(key, StringComparison.Ordinal, out var value))
            {
                ret = value.GetValue<T>();
                return true;
            }

            ret = default;
            return false;
        }

        public static T GetEnum<T>(this JToken value) where T : struct
        {
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
        
        public static T GetEnum<T>(this JObject data, string key) where T : struct
        {
            if (data == null || !data.TryGetValue(key, StringComparison.Ordinal, out var value))
            {
                return default;
            }

            return value.GetEnum<T>();
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

            return value.TryGetEnum(out ret);
        }

        public static bool TryGetEnum<T>(this JToken value, out T ret) where T : struct
        {
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
        
        public static Vector3 GetVector3(this JObject token, string key)
        {
            if (!token.TryGetValue(key, out var vectorToken) || !(vectorToken is JObject vectorObject))
            {
                return default;
            }

            float y = default;
            float z = default;
                
            var b = vectorObject.TryGetValue("x", out float x);
            b = b && vectorObject.TryGetValue("y", out y);
            b = b && vectorObject.TryGetValue("z", out z);

            return !b ? default : new Vector3(x, y, z);
        }
        
        public static bool TryGetVector(this JObject token, string key, out Vector3 ret)
        {
            if (token.TryGetValue(key, out var vectorToken) &&
                vectorToken is JObject vectorObject)
            {
                float y = default;
                float z = default;
                
                var b = vectorObject.TryGetValue("x", out float x);
                b = b && vectorObject.TryGetValue("y", out y);
                b = b && vectorObject.TryGetValue("z", out z);

                if (b)
                {
                    ret = new Vector3(x, y, z);
                    return true;
                }
            }

            ret = default;
            return false;
        }
        
        public static Vector2 GetVector2(this JObject token, string key)
        {
            if (!token.TryGetValue(key, out var vectorToken) || !(vectorToken is JObject vectorObject))
            {
                return default;
            }

            float y = default;
                
            var b = vectorObject.TryGetValue("x", out float x);
            b = b && vectorObject.TryGetValue("y", out y);

            return !b ? default : new Vector2(x, y);
        }
        
        public static bool TryGetVector(this JObject token, string key, out Vector2 ret)
        {
            if (token.TryGetValue(key, out var vectorToken) &&
                vectorToken is JObject vectorObject)
            {
                float y = default;

                var b = vectorObject.TryGetValue("x", out float x);
                b = b && vectorObject.TryGetValue("y", out y);

                if (b)
                {
                    ret = new Vector2(x, y);
                    return true;
                }
            }

            ret = default;
            return false;
        }
        
        public static Vector2 GetSize(this JObject data, string key)
        {
            if (!data.TryGetValue(key, out var vectorToken) || !(vectorToken is JObject vectorObject))
            {
                return default;
            }

            float height = default;
                
            var b = vectorObject.TryGetValue("width", out float width);
            b = b && vectorObject.TryGetValue("height", out height);

            return b ? new Vector2(width, height) : default;
        }
        
        public static bool TryGetSize(this JObject data, string key, out Vector2 ret)
        {
            if (data.TryGetValue(key, out var vectorToken) &&
                vectorToken is JObject vectorObject)
            {
                float height = default;
                
                var b = vectorObject.TryGetValue("width", out float width);
                b = b && vectorObject.TryGetValue("height", out height);

                if (b)
                {
                    ret = new Vector2(width, height);
                    return true;
                }
            }

            ret = default;
            return false;
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