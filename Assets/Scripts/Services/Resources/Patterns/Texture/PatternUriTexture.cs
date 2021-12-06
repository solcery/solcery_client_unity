using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.Resources.Patterns.Texture
{
    public sealed class PatternUriTexture : IPattern
    {
        PatternTypes IPattern.PatternType => PatternTypes.UriTexture;

        public static IPattern Create()
        {
            return new PatternUriTexture();
        }

        private PatternUriTexture() { }

        bool IPattern.HasPattern(JObject json)
        {
            return json.TryGetValue("picture", out string picture)
                   && picture != null;
        }

        PatternData IPattern.GetPatternData(JObject json)
        {
            return PatternUriTextureData.Create(json);
        }

        void IPattern.Cleanup() { }
    }
}