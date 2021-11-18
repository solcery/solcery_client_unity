using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.Resources.Patterns.Texture
{
    public sealed class PatternUriTextureData : PatternData
    {
        public readonly string Uri;

        public static PatternUriTextureData Create(JObject json)
        {
            return new PatternUriTextureData(json);
        }
        
        private PatternUriTextureData(JObject json)
        {
            Uri = json.GetValue<string>("picture");
        }

        public override void Cleanup() { }
    }
}