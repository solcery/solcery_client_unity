using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Utils
{
    public static class BrickUtils
    {
        public static bool TryGetBrickTypeSubType(JToken token, out Tuple<int, int> typeSupType)
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

        public static bool TryGetBrickParameters(JToken token, out JArray brickParameters)
        {
            brickParameters = null;
            return token is JObject obj && obj.TryGetValue("params", out brickParameters);
        }
    }
}