using Newtonsoft.Json.Linq;

namespace Solcery.Services.Resources.Patterns
{
    public interface IPattern
    {
        PatternTypes PatternType { get; }
        bool HasPattern(JObject json);
        PatternData GetPatternData(JObject json);
        void Cleanup();
    }
}