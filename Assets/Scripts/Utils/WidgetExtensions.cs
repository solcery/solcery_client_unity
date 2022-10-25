using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Services.GameContent.Items;
using Solcery.Widgets_new.Eclipse;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using TMPro;
using UnityEngine;

namespace Solcery.Utils
{
    public static class WidgetExtensions
    {
        public static bool TryGetTokenFromPosition(EcsWorld world, int fromPlaceId, int fromCardId, int slotId, out Vector3 position)
        {
            var placeWidget = world.GetPlaceWidget(fromPlaceId);
            if (placeWidget != null && placeWidget is IPlaceWidgetTokenCollection widget)
            {
                if (widget.TryGetTokenPosition(world, fromCardId, slotId, out position))
                {
                    return true;
                }
            }
            
            position = Vector3.zero;
            return false;
        }
        
        public static void UpdateFontSize(this TMP_Text text, float fontSize)
        {
            var autoSizeText = fontSize == 0f;
            text.enableAutoSizing = autoSizeText;
            if (!autoSizeText)
            {
                text.fontSize = fontSize;
            }
        }

        public static void UpdateHighlights(this IWidgetLayoutHighlight widgetLayout, Dictionary<string, IAttributeValue> attributes)
        {
            var animHighlightActive = attributes.TryGetValue(GameJsonKeys.AnimHighlight, out var animHighlightAttribute) && animHighlightAttribute.Current > 0;
            foreach (var image in widgetLayout.Highlights)
            {
                image.gameObject.SetActive(animHighlightActive);
            }
            
            if (animHighlightActive)
            {
                var r = attributes.TryGetValue(GameJsonKeys.AnimHighlightColorR, out var animHighlightColorRAttribute)  ? animHighlightColorRAttribute.Current : 255;
                var g = attributes.TryGetValue(GameJsonKeys.AnimHighlightColorG, out var animHighlightColorGAttribute)  ? animHighlightColorGAttribute.Current : 255;
                var b = attributes.TryGetValue(GameJsonKeys.AnimHighlightColorB, out var animHighlightColorBAttribute)  ? animHighlightColorBAttribute.Current : 255;
                var a = attributes.TryGetValue(GameJsonKeys.AnimHighlightColorA, out var animHighlightColorAAttribute)  ? animHighlightColorAAttribute.Current : 255;
                var color = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
                foreach (var image in widgetLayout.Highlights)
                {
                    image.color = color;
                }
            }
        }
    }
}