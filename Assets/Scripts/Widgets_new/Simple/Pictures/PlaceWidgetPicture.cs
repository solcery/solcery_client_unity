using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;
using Solcery.Widgets.Canvas;

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

        public override void Update(EcsWorld world, int[] entityIds)
        {
            if (entityIds.Length <= 0)
            {
                return;
            }

            var entityId = entityIds[0];
            
            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            foreach (var objectTypesEntityId in objectTypesFilter)
            {
                var objectTypes = world.GetPool<ComponentObjectTypes>().Get(objectTypesEntityId).Types;
                if (!objectTypePool.Has(entityId))
                {
                    break;
                }

                if (objectTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var objectTypeDataObject) 
                    && objectTypeDataObject.TryGetValue("picture", out string picture)
                    && Game.ServiceResource.TryGetTextureForKey(picture, out var texture))
                {
                    Layout.UpdatePicture(texture);
                }
            }
        }
    }
}