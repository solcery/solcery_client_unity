using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Utils;
using Solcery.Widgets_new.Attributes.Enum;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets_new.Factories
{
    public sealed class PlaceWidgetFactory : IPlaceWidgetFactory
    {
        private IGame _game;
        private IWidgetCanvas _widgetCanvas;
        private Dictionary<PlaceWidgetTypes, Func<IWidgetCanvas, IGame, string, JObject, PlaceWidget>> _placeWidgetCreators;

        public static IPlaceWidgetFactory Create(IGame game, IWidgetCanvas widgetCanvas)
        {
            return new PlaceWidgetFactory(game, widgetCanvas);
        }

        private PlaceWidgetFactory(IGame game, IWidgetCanvas widgetCanvas)
        {
            _game = game;
            _widgetCanvas = widgetCanvas;
            _placeWidgetCreators = new Dictionary<PlaceWidgetTypes, Func<IWidgetCanvas, IGame, string, JObject, PlaceWidget>>();
        }

        void IPlaceWidgetFactory.RegistrationPlaceWidget(PlaceWidgetTypes placeWidgetType, Func<IWidgetCanvas, IGame,  string, JObject, PlaceWidget> createdFunc)
        {
            if (_placeWidgetCreators.ContainsKey(placeWidgetType))
            {
                return;
            }
            
            _placeWidgetCreators.Add(placeWidgetType, createdFunc);
        }

        bool IPlaceWidgetFactory.TryCreatePlaceWidgetByType(JObject placeDataObject, out PlaceWidget placeWidget)
        {
            placeWidget = null;

            var placeWidgetType = placeDataObject.TryGetEnum("layout", out PlaceWidgetTypes pwt)
                ? pwt
                : PlaceWidgetTypes.Stacked;
            
            if (!_placeWidgetCreators.ContainsKey(placeWidgetType))
            {
                return false;
            }

            placeWidget = _placeWidgetCreators[placeWidgetType].Invoke(_widgetCanvas, _game,
                EnumPlaceWidgetPrefabPathAttribute.GetPrefabPath(placeWidgetType), placeDataObject);
            return true;
        }

        void IPlaceWidgetFactory.Destroy()
        {
            Cleanup();

            _game = null;
            _widgetCanvas = null;
            _placeWidgetCreators = null;
        }

        private void Cleanup()
        {
            _placeWidgetCreators.Clear();
        }
    }
}