using Newtonsoft.Json.Linq;
using Solcery.Types;
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

        public static bool Overlaps(this WorldRect rect1, WorldRect rect2)
        {
            return rect2.xMax > rect1.xMin && rect2.xMin < rect1.xMax && rect2.yMax > rect1.yMin && rect2.yMin < rect1.yMax;
        }
    }
}