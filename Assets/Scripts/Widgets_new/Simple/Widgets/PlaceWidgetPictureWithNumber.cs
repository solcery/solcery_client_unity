using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;

namespace Solcery.Widgets_new.Simple.Widgets
{
    public sealed class PlaceWidgetPictureWithNumber : PlaceWidget<PlaceWidgetPictureWithNumberLayout>
    {
        private string _lastPictureName;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetPictureWithNumber(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetPictureWithNumber(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _lastPictureName = "";
        }

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
                var tplid = objectTypePool.Get(entityId).TplId;
                if (Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid)
                    && itemType.TryGetValue(out var valueToken, "picture", id))
                {
                    var picture = valueToken.GetValue<string>();
                    if (_lastPictureName != picture
                        && Game.ServiceResource.TryGetTextureForKey(picture, out var texture))
                    {
                        _lastPictureName = picture;
                        Layout.UpdateImage(texture);
                    }
                }
            }

            var objectAttributesPool = world.GetPool<ComponentObjectAttributes>();
            if (objectAttributesPool.Has(entityId)
                && objectAttributesPool.Get(entityId).Attributes.TryGetValue("number", out var number))
            {

                Layout.UpdateText(number.Current.ToString());
                var diff =  number.Current - number.Old;
                if (number.Changed)
                {
                    Layout.ShowDiff(diff);
                }
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            return Layout;
        }
    }
}