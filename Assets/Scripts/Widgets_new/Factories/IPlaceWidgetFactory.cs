using System;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets_new.Factories
{
    public interface IPlaceWidgetFactory
    {
        void RegistrationPlaceWidget(PlaceWidgetTypes placeWidgetType, Func<IWidgetCanvas, IGame,  string, JObject, PlaceWidget> createdFunc);
        bool TryCreatePlaceWidgetByType(JObject placeDataObject, out PlaceWidget placeWidget);
        void Destroy();
    }
}