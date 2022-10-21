namespace Solcery.Games
{
    public static class GameJsonKeys
    {
        // Commons
        public static readonly string Picture = "picture";
        public static readonly string AnimHighlight = "anim_highlight";
        public static readonly string AnimHighlightColorR = "anim_highlight_r";
        public static readonly string AnimHighlightColorG = "anim_highlight_g";
        public static readonly string AnimHighlightColorB = "anim_highlight_b";
        public static readonly string AnimDestroy = "anim_destroy";
        public static readonly string AnimDestroyTime = "anim_card_destroy_time";

        // Widgets
        public static readonly string WidgetPictureType = "picture_type";
        public static readonly string WidgetPicturePixelsPerUnitMultiplier = "picture_pixels_per_unit_multiplier";
        
        // Card
        public static readonly string CardTypes = "card_types";
        public static readonly string CardType = "type";
        public static readonly string CardDisplayedType = "displayed_type";
        public static readonly string CardTypeFontSize = "type_font_size";
        public static readonly string CardDisplayedName = "displayed_name";
        public static readonly string CardNameFontSize = "name_font_size";
        public static readonly string CardDescription = "description";
        public static readonly string CardShowDescription = "show_description";
        public static readonly string CardTokenSlots = "token_slots";
        public static readonly string CardTimerText = "timer_text";
        public static readonly string CardTooltipId = "tooltip_id";
        public static readonly string CardShowDuration = "show_duration";
        public static readonly string CardDuration = "duration";
        public static readonly string CardAnimCardFly = "anim_card_fly";
        public static readonly string CardAnimCardFlyFromPlace = "anim_card_fly_from_place";
        public static readonly string CardAnimCardFlyTime = "anim_card_fly_time";
        public static readonly string CardDescriptionFontSize = "description_font_size";
        public static readonly string CardDefaultTimerValue = "default_timer_value";
        
        // Token
        public static readonly string TokenSlot = "slot";
        
        // Place
        public static readonly string PlaceId = "place_id";
        public static readonly string PlaceInteractableForActiveLocalPlayer = "interactable_for_active_local_player";
        public static readonly string PlaceZOrder = "z_order";
        public static readonly string PlaceFillColor = "fill_color";
        public static readonly string PlaceShowFrame = "show_frame";
        public static readonly string PlaceFrameColor = "frame_color";
        public static readonly string PlaceTooltipId = "tooltip_id";
        public static readonly string PlaceCaption = "caption";
        public static readonly string PlaceCaptionSize = "caption_size";
        public static readonly string PlaceCaptionPosition = "caption_position";
        public static readonly string PlaceCaptionColor = "caption_color";
        public static readonly string PlaceX1 = "x1";
        public static readonly string PlaceX2 = "x2";
        public static readonly string PlaceY1 = "y1";
        public static readonly string PlaceY2 = "y2";

        // Tooltip
        public static readonly string TooltipText = "text";
        public static readonly string TooltipDelay = "font_size";
        public static readonly string TooltipFillColor = "fill_color";
        public static readonly string TooltipFontSize = "font_size";
        public static readonly string TooltipCardTypeId = "card_type_id";
        
        // Global Card Attribute
        public static readonly string GlobalCardAttributes = "card_attributes";
        public static readonly string GlobalCardAttributeCode = "code";
        
        // Bricks
        public static readonly string GlobalCustomBricks = "custom_bricks";
        public static readonly string VisibilityConditionBrick = "visibility_condition";
        public static readonly string AvailabilityConditionBrick = "availability_condition";
        
        // Full card
        public static readonly string CardDescriptionFontSizeFull = "description_font_size_full";
        public static readonly string CardNameFontSizeFull = "name_font_size_full";
        public static readonly string CardTypeFontSizeFull = "type_font_size_full";
        
        // Picture with number
        public static readonly string PictureNumber = "number";
        public static readonly string PictureAnimNumber = "anim_number";
    }
}