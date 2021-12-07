using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets_new.Simple.Button
{
    public sealed class PlaceWidgetButton : PlaceWidget<PlaceWidgetButtonLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetButton(widgetCanvas, serviceResource, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetButton(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, serviceResource, prefabPathKey, placeDataObject) { }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            
        }
    }
}