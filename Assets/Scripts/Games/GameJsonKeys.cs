namespace Solcery.Games
{
    public static class GameJsonKeys
    {
        /// Card
        public static readonly string CardTypes = "card_types";
        public static readonly string CardTooltipId = "tooltip_id";
        
        // Place
        public static readonly string PlaceId = "place_id";
        public static readonly string PlaceInteractableForActiveLocalPlayer = "interactable_for_active_local_player";
        public static readonly string PlaceZOrder = "z_order";
        public static readonly string PlaceFillColor = "fill_color";
        public static readonly string PlaceShowFrame = "show_frame";
        public static readonly string PlaceFrameColor = "frame_color";
        public static readonly string PlaceTooltipId = "tooltip_id";
        public static readonly string PlaceCaption = "caption";
        public static readonly string PlaceCaptionColor = "caption_color";
        
        // Tooltip
        public static readonly string TooltipText = "text";
        public static readonly string TooltipDelay = "font_size";
        public static readonly string TooltipFillColor = "fill_color";
        public static readonly string TooltipFontSize = "font_size";
        
        // Global Card Attribute
        public static readonly string GlobalCardAttributes = "card_attributes";
        public static readonly string GlobalCardAttributeCode = "code";
        
        // Custom Bricks
        public static readonly string GlobalCustomBricks = "custom_bricks";
    }
}