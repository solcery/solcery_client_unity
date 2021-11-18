using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.Resources.Patterns.Widgets.Picture
{
    public class PatternPicture : IPattern
    {
        PatternTypes IPattern.PatternType => PatternTypes.WidgetPicture;

        public static IPattern Create()
        {
            return new PatternPicture();
        }

        private PatternPicture() { }

        bool IPattern.HasPattern(JObject json)
        {
            return json.HasKey("name")
                   && json.HasKey("description")
                   && !json.HasKey("action")
                   && json.HasKey("picture");
        }

        PatternData IPattern.GetPatternData(JObject json)
        {
            return PatternWidgetData.Create("ui/picture");
        }

        void IPattern.Cleanup() { }
    }
}