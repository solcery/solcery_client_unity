using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Objects;
using Solcery.Services.Resources;
using Solcery.Utils;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets_new.Simple.Title
{
    public sealed class PlaceWidgetTitle : PlaceWidget<PlaceWidgetTitleLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetTitle(widgetCanvas, serviceResource, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetTitle(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, serviceResource, prefabPathKey, placeDataObject) { }
        
        public override void Update(EcsWorld world, int[] entityIds)
        {
            if (entityIds.Length <= 0)
            {
                Layout.UpdateTitle("No cards in this place.");
            }
            
            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            foreach (var entityId in objectTypesFilter)
            {
                var objectTypes = world.GetPool<ComponentObjectTypes>().Get(entityId).Types;
                if (!objectTypePool.Has(entityIds[0]))
                {
                    break;
                }

                if (objectTypes.TryGetValue(objectTypePool.Get(entityIds[0]).Type, out var objectTypeDataObject) 
                    && objectTypeDataObject.TryGetValue("description", out string description))
                {
                    Layout.UpdateTitle(description);
                    return;
                }
                
                break;
            }
            
            Layout.UpdateTitle("No card type data.");
        }
    }
}