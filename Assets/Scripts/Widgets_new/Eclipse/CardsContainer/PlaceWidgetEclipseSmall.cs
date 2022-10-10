using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Canvas;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public class PlaceWidgetEclipseSmall : PlaceWidgetEclipse
    {
        public new static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
        {
            return new PlaceWidgetEclipseSmall(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetEclipseSmall(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            CardsPool = Game.EclipseCardSmallInContainerWidgetPool;
        }
    }
}
