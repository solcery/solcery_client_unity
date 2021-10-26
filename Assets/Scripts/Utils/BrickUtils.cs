using Newtonsoft.Json.Linq;

namespace Solcery.Utils
{
    public static class BrickUtils
    {
        public static bool TryGetBrickTypeName(JToken token, out string brickTypeName)
        {
            brickTypeName = null;
            return token is JObject obj && obj.TryGetValue("type", out brickTypeName);
        }

        public static bool TryGetBrickParameters(JToken token, out JArray brickParameters)
        {
            brickParameters = null;
            return token is JObject obj && obj.TryGetValue("params", out brickParameters);
        }
    }
}