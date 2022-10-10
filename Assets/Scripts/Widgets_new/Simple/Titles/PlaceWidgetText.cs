using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;

namespace Solcery.Widgets_new.Simple.Titles
{
    public sealed class PlaceWidgetText : PlaceWidget<PlaceWidgetTextLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetText(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetText(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject) { }
        
        public override void Update(EcsWorld world, bool isVisible, bool isAvailable, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0 && isVisible);
            
            if (entityIds.Length <= 0 || !isVisible)
            {
                Layout.UpdateTitle("No cards in this place.");
                return;
            }

            var entityId = entityIds[0];
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            if (objectTypePool.Has(entityId)
                && objectIdPool.Has(entityId))
            {
                var id = objectIdPool.Get(entityId).Id;
                var tplid = objectTypePool.Get(entityId).TplId;
                if (Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid)
                    && itemType.TryGetValue(out var valueToken, "description", id))
                {
                    var description = valueToken.GetValue<string>();
                    Layout.UpdateTitle(description);
                    return;
                }
            }

            Layout.UpdateTitle("No card type data.");
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            return Layout;
        }
    }
}