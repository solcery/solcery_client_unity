using Solcery.Widgets_new.Attributes.Enum;

namespace Solcery.Widgets_new
{
    public enum PlaceWidgetTypes
    {
        [EnumPlaceWidgetPrefabPath("")]
        None = -10,
        [EnumPlaceWidgetPrefabPath("ui/ui_tooltip")]
        Tooltip = -5,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_card")]
        EclipseCard = -4,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_token")]
        EclipseToken = -3,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_list_tokens")]
        EclipseListTokens = -2,
        [EnumPlaceWidgetPrefabPath("ui/ui_card")]
        Card = -1,
        [EnumPlaceWidgetPrefabPath("ui/ui_stack")]
        Stacked = 0,
        [EnumPlaceWidgetPrefabPath("ui/ui_hand")]
        LayedOut = 1,
        [EnumPlaceWidgetPrefabPath("ui/ui_picture_with_number")]
        PictureWithNumber = 2,
        [EnumPlaceWidgetPrefabPath("ui/ui_text")]
        Text = 3,
        [EnumPlaceWidgetPrefabPath("ui/ui_button")]
        Button = 4,
        [EnumPlaceWidgetPrefabPath("ui/ui_picture")]
        Picture = 5,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_event_tracker_one_card")]
        EclipseOneCard = 6,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_card_full")]
        EclipseOneCardFull = 7,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_token_storage")]
        EclipseTokenStorage = 8,
        [EnumPlaceWidgetPrefabPath("ui/ui_eclipse_event_tracker")]
        EclipseEventTracker = 9
    }
}