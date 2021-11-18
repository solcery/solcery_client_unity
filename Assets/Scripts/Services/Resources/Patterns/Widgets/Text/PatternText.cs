using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.Resources.Patterns.Widgets.Text
{
    public sealed class PatternText : IPattern
    {
        PatternTypes IPattern.PatternType => PatternTypes.WidgetText;

        public static IPattern Create()
        {
            return new PatternText();
        }

        private PatternText() { }

        bool IPattern.HasPattern(JObject json)
        {
            return json.HasKey("name")
                   && json.HasKey("description")
                   && !json.HasKey("action")
                   && !json.HasKey("picture");
        }

        PatternData IPattern.GetPatternData(JObject json)
        {
            return PatternWidgetData.Create("ui/text");
        }

        void IPattern.Cleanup() { }
    }
}