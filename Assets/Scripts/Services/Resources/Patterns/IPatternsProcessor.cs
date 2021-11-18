using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.Resources.Patterns
{
    public interface IPatternsProcessor
    {
        void PatternRegistration(IPattern pattern);
        void ProcessGameContent(JObject gameContentData);
        bool TryGetAllPatternDataForType(PatternTypes patternType, out List<PatternData> patternDataList);
        void Cleanup();
    }
}