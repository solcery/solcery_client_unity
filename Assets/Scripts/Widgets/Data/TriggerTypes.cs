using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Solcery.Widgets.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerTypes
    {
        None,
        [EnumMember(Value = "on_click")]
        OnClick,
        [EnumMember(Value = "on_attribute_change")]
        OnAttributeChange
    }
}