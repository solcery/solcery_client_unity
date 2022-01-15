using System;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Runtime.Utils
{
    public static class BrickUtils
    {
        public static bool TryGetBrickTypeSubType(this JToken token, out Tuple<int, int> typeSupType)
        {
            typeSupType = null;

            if (token is JObject obj 
                && obj.TryGetValue("type", out int type)
                && obj.TryGetValue("subtype", out int subType))
            {
                typeSupType = new Tuple<int, int>(type, subType);
                return true;
            }
            
            return false;
        }

        public static bool TryGetBrickParameters(this JToken token, out JArray brickParameters)
        {
            brickParameters = null;
            return token is JObject obj && obj.TryGetValue("params", out brickParameters);
        }

        public static bool TryParseBrickParameter<T>(this JToken token, out string name, out T value)
        {
            name = null;
            value = default;
            return token is JObject obj && obj.TryGetValue("name", out name) && obj.TryGetValue("value", out value);
        }
    }
}