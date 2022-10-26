using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Services.Events;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;

namespace Solcery.Widgets_new.Simple.Buttons
{
    public sealed class PlaceWidgetButton : PlaceWidget<PlaceWidgetButtonLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetButton(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetButton(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject) { }

        public override void Update(EcsWorld world, bool isVisible, bool isAvailable, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0 && isVisible);
            Layout.ClearAllOnClickListener();
            
            if (entityIds.Length <= 0 || !isVisible)
            {
                return;
            }

            var entityId = entityIds[0];
            var objectIdPool = world.GetPool<ComponentObjectId>();

            if (!objectIdPool.Has(entityId))
            {
                return;
            }
            
            Layout.AddOnClickListener(() =>
            {
                ServiceEvents.Current.BroadcastEvent(OnLeftClickEventData.Create(entityId));
            });

            var objectTypePool = world.GetPool<ComponentObjectType>();
            if (objectTypePool.Has(entityId))
            {
                if (Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, objectTypePool.Get(entityId).TplId))
                {
                    UpdateItemType(itemType, objectIdPool.Get(entityId).Id);
                    UpdateAttributes(world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes);
                }
            }
        }

        private void UpdateItemType(IItemType itemType, int objectId)
        {
            var displayedName = itemType.TryGetValue(out var nameToken, GameJsonKeys.CardDisplayedName, objectId)
                ? nameToken.GetValue<string>()
                : string.Empty;
            var nameFontSize = itemType.TryGetValue(out var valueNameFontSizeAttributeToken, GameJsonKeys.CardNameFontSize, objectId)
                ? valueNameFontSizeAttributeToken.GetValue<float>()
                : 0f;
            Layout.UpdateButtonText(displayedName, nameFontSize);
        }

        private void UpdateAttributes(Dictionary<string, IAttributeValue> attributes)
        {
            Layout.UpdateHighlights(attributes);
        }
        
        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            return Layout;
        }
    }
}