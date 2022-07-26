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
                Layout.UpdateButtonText("No card id.");
                return;
            }
            
            Layout.AddOnClickListener(() =>
            {
                ServiceEvents.Current.BroadcastEvent(OnLeftClickEventData.Create(entityId));
            });
            
            var objectTypePool = world.GetPool<ComponentObjectType>();
            if (objectTypePool.Has(entityId))
            {
                var tplid = objectTypePool.Get(entityId).Type;
                if (Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid)
                    && itemType.TryGetValue(out var valueToken, "name", entityId))
                {
                    var name = valueToken.GetValue<string>();
                    Layout.UpdateButtonText(name);
                    return;
                }
            }
            
            Layout.UpdateButtonText("No card type data.");
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            return Layout;
        }
    }
}