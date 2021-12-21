using System;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;

namespace Solcery.Widgets_new.Factories
{
    public interface IPlaceWidgetFactory
    {
        ICardInContainerPool CardInContainerPool { get; }

        void RegistrationPlaceWidget(PlaceWidgetTypes placeWidgetType, Func<IWidgetCanvas, IGame,  string, JObject, PlaceWidget> createdFunc);
        bool TryCreatePlaceWidgetByType(JObject placeDataObject, out PlaceWidget placeWidget);
        void Destroy();
    }
}