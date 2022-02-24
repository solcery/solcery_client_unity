using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Utils
{
    public static class TypesUtils
    {
        public static JObject ToJObject(this Vector2 vector)
        {
            return new JObject
            {
                {"x", new JValue(vector.x)},
                {"y", new JValue(vector.y)},
            };
        }
        
        public static JObject ToJObject(this Vector3 vector)
        {
            return new JObject
            {
                {"x", new JValue(vector.x)},
                {"y", new JValue(vector.y)},
                {"z", new JValue(vector.z)}
            };
        }
    }
}