using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using UnityEngine.UI;

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
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            if (objectTypePool.Has(entityId)
                && objectIdPool.Has(entityId))
            {
                var id = objectIdPool.Get(entityId).Id;
                var tplId = objectTypePool.Get(entityId).TplId;
                if (Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplId)
                    && itemType.TryGetValue(out var valueToken, GameJsonKeys.Picture, id)
                    && Game.ServiceResource.TryGetTextureForKey(valueToken.GetValue<string>(), out var texture))
                {
                    Layout.UpdatePicture(texture);
                    if (itemType.TryGetValue(out var pictureTypeToken, GameJsonKeys.WidgetPictureType, id))
                    {
                        var pixelsPerUnitMultiplier = itemType.TryGetValue(out var picturePixelsPerUnitMultiplierToken, GameJsonKeys.WidgetPicturePixelsPerUnitMultiplier, id)
                            ? picturePixelsPerUnitMultiplierToken.GetValue<float>()
                            : 1;
                        Layout.UpdatePictureType(pictureTypeToken.GetValue<Image.Type>(), pixelsPerUnitMultiplier);
                    }
                }
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            return Layout;
        }
    }
}