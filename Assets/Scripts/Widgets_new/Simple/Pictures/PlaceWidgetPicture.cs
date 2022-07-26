using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;

namespace Solcery.Widgets_new.Simple.Pictures
{
    public sealed class PlaceWidgetPicture : PlaceWidget<PlaceWidgetPictureLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetPicture(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetPicture(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject) { }

        public override void Update(EcsWorld world, bool isVisible, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0 && isVisible);
            
            if (entityIds.Length <= 0 || !isVisible)
            {
                return;
            }

            var entityId = entityIds[0];
            var objectTypePool = world.GetPool<ComponentObjectType>();
            if (objectTypePool.Has(entityId))
            {
                var tplid = objectTypePool.Get(entityId).Type;
                if (Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid)
                    && itemType.TryGetValue(out var valueToken, "picture", entityId)
                    && Game.ServiceResource.TryGetTextureForKey(valueToken.GetValue<string>(), out var texture))
                {
                    Layout.UpdatePicture(texture);
                }
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            return Layout;
        }
    }
}