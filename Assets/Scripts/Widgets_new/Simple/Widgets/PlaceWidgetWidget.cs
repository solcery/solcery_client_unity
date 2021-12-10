using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets_new.Simple.Widgets
{
    public sealed class PlaceWidgetWidget : PlaceWidget<PlaceWidgetWidgetLayout>
    {
        private string _lastPictureName;
        private int? _lastNumber;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetWidget(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetWidget(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _lastPictureName = "";
            _lastNumber = null;
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0);
            
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
                    && _lastPictureName != picture
                    && Game.ServiceResource.TryGetTextureForKey(picture, out var texture))
                {
                    _lastPictureName = picture;
                    Layout.UpdateImage(texture);
                }
            }

            var objectAttributesPool = world.GetPool<ComponentObjectAttributes>();
            if (objectAttributesPool.Has(entityId)
                && objectAttributesPool.Get(entityId).Attributes.TryGetValue("number", out var number))
            {
                var diff =  number - _lastNumber?? 0;
                Layout.UpdateText(number.ToString());
                if (diff != 0)
                {
                    Layout.ShowDiff(diff);
                }
                _lastNumber = number;
            }
        }
    }
}