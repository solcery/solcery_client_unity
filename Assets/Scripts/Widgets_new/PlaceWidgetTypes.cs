using Solcery.Widgets_new.Attributes.Enum;

namespace Solcery.Widgets_new
{
    public enum PlaceWidgetTypes
    {
        [EnumPlaceWidgetPrefabPath("")]
        None = -10,
        [EnumPlaceWidgetPrefabPath("ui/ui_card")]
        Card = -1,
        [EnumPlaceWidgetPrefabPath("ui/ui_stack")]
        Stacked = 0,
        [EnumPlaceWidgetPrefabPath("ui/ui_hand")]
        LayedOut = 1,
        [EnumPlaceWidgetPrefabPath("ui/ui_widget")]
        Widget = 2,
        [EnumPlaceWidgetPrefabPath("ui/ui_title")]
        Title = 3,
        [EnumPlaceWidgetPrefabPath("ui/ui_button")]
        Button = 4,
        [EnumPlaceWidgetPrefabPath("ui/ui_picture")]
        Picture = 5,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_hand")]
        Eclipse = 6,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_tokens_stockpile")]
        TokensStockpile = 7
    }
}