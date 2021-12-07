using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
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
            : base(widgetCanvas, serviceResource, placeDataObject, prefabPathKey) { }
        
        public override void Update(EcsWorld world, int[] entityIds)
        {
            
        }
    }
}