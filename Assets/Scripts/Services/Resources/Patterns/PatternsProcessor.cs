using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Utils;

namespace Solcery.Services.Resources.Patterns
{
    public sealed class PatternsProcessor : IPatternsProcessor
    {
        private Dictionary<PatternTypes, IPattern> _patterns;
        private Dictionary<PatternTypes, List<PatternData>> _processedPatternData;

        public static IPatternsProcessor Create()
        {
            return new PatternsProcessor();
        }

        private PatternsProcessor()
        {
            _patterns = new Dictionary<PatternTypes, IPattern>();
            _processedPatternData = new Dictionary<PatternTypes, List<PatternData>>();
        }

        public void PatternRegistration(IPattern pattern)
        {
            _patterns.Add(pattern.PatternType, pattern);
        }

        public void ProcessGameContent(JObject gameContentData)
        {
            var objects = gameContentData.GetValue<JObject>(GameJsonKeys.CardTypes).GetValue<JArray>("objects");
            foreach (var objectToken in objects)
            {
                if (objectToken is JObject objectData)
                {
                    foreach (var pattern in _patterns)
                    {
                        if (!pattern.Value.HasPattern(objectData))
                        {
                            continue;
                        }

                        if (!_processedPatternData.ContainsKey(pattern.Key))
                        {
                            _processedPatternData.Add(pattern.Key, new List<PatternData>());
                        }
                        
                        _processedPatternData[pattern.Key].Add(pattern.Value.GetPatternData(objectData));
                    }
                }
            }
        }

        public bool TryGetAllPatternDataForType(PatternTypes patternType, out List<PatternData> patternDataList)
        {
            return _processedPatternData.TryGetValue(patternType, out patternDataList);
        }

        public void Cleanup()
        {
            foreach (var pattern in _patterns)
            {
                pattern.Value.Cleanup();
            }
            _patterns.Clear();
            _patterns = null;

            foreach (var patternsData in _processedPatternData)
            {
                foreach (var patternData in patternsData.Value)
                {
                    patternData.Cleanup();
                }
                patternsData.Value.Clear();
            }
            _processedPatternData.Clear();
            _processedPatternData = null;
        }
    }
}