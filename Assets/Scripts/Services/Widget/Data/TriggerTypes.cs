using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Solcery.Services.Widget
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerTypes
    {
        None,
        [EnumMember(Value = "on_click")]
        OnClick
    }
}