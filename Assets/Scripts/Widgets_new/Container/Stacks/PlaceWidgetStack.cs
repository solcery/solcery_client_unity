using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStack : PlaceWidget<PlaceWidgetStackLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetStack(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetStack(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject) { }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0);
            
            if (entityIds.Length <= 0)
            {
                return;
            }
        }
    }
}