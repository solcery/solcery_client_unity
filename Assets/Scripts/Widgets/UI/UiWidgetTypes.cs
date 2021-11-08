using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Solcery.Widgets.UI
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UiWidgetTypes
    {
        [EnumMember(Value = "canvas")]
        Canvas,
        [EnumMember(Value = "Button")]
        Button
    }
}