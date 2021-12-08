using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Attributes.Enum;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets_new
{
    public sealed class PlaceWidgetFactory
    {
        private Dictionary<PlaceWidgetTypes, Func<IWidgetCanvas, IGame, string, JObject, PlaceWidget>> _placeWidgetCreators;

        public void RegistrationPlaceWidget(PlaceWidgetTypes placeWidgetType, Func<IWidgetCanvas, IGame,  string, JObject, PlaceWidget> createdFunc)
        {
            if (_placeWidgetCreators.ContainsKey(placeWidgetType))
            {
                return;
            }
            
            _placeWidgetCreators.Add(placeWidgetType, createdFunc);
        }

        public bool TryCreatePlaceWidgetByType(PlaceWidgetTypes placeWidgetType, IWidgetCanvas widgetCanvas, IGame game, JObject placeDataObject, out PlaceWidget placeWidget)
        {
            placeWidget = null;
            if (!_placeWidgetCreators.ContainsKey(placeWidgetType))
            {
                return false;
            }

            placeWidget = _placeWidgetCreators[placeWidgetType].Invoke(widgetCanvas, game,
                EnumPlaceWidgetPrefabPathAttribute.GetPrefabPath(placeWidgetType), placeDataObject);
            return true;
        } 
    }
}