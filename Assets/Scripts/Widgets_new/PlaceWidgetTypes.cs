using Solcery.Widgets_new.Attributes.Enum;

namespace Solcery.Widgets_new
{
    public enum PlaceWidgetTypes
    {
        [EnumPlaceWidgetPrefabPath("")]
        None = -1,
        [EnumPlaceWidgetPrefabPath("ui/stack")]
        Stacked = 0,
        [EnumPlaceWidgetPrefabPath("ui/deck")]
        LayedOut = 1,
        [EnumPlaceWidgetPrefabPath("ui/widget")]
        Widget = 2,
        [EnumPlaceWidgetPrefabPath("ui/title")]
        Title = 3,
        [EnumPlaceWidgetPrefabPath("ui/button")]
        Button = 4,
        [EnumPlaceWidgetPrefabPath("ui/picture")]
        Picture = 5
    }
}