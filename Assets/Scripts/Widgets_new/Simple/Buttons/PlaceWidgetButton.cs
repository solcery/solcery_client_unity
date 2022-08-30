using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Services.Events;
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

        public override void Update(EcsWorld world, bool isVisible, int[] entityIds)
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
            if (objectTypePool.Has(entityId)
                && objectIdPool.Has(entityId))
            {
                var objectId = objectIdPool.Get(entityId).Id;
                var tplid = objectTypePool.Get(entityId).TplId;
                if (Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid))
                {
                    var displayedName = itemType.TryGetValue(out var nameToken, GameJsonKeys.CardDisplayedName, objectId)
                        ? nameToken.GetValue<string>()
                        : string.Empty;
                    var nameFontSize = itemType.TryGetValue(out var valueNameFontSizeAttributeToken, GameJsonKeys.CardNameFontSize, objectId)
                        ? valueNameFontSizeAttributeToken.GetValue<float>()
                        : 0f;
                    Layout.UpdateButtonText(displayedName, nameFontSize);
                }
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            return Layout;
        }
    }
}