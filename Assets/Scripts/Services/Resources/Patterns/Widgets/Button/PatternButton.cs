using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.Resources.Patterns.Widgets.Button
{
    public sealed class PatternButton : IPattern
    {
        PatternTypes IPattern.PatternType => PatternTypes.WidgetButton;

        public static IPattern Create()
        {
            return new PatternButton();
        }

        private PatternButton() { }

        bool IPattern.HasPattern(JObject json)
        {
            return json.HasKey("name")
                   && json.HasKey("description")
                   && json.HasKey("action")
                   && !json.HasKey("picture");
        }

        PatternData IPattern.GetPatternData(JObject json)
        {
            return PatternWidgetData.Create("ui/button");
        }

        void IPattern.Cleanup() { }
    }
}