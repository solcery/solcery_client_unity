using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Utils;
using Solcery.Widgets.Area;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Deck;
using Solcery.Widgets.Stack;

namespace Solcery.Widgets.Factory
{
    public sealed class WidgetFactory : IWidgetFactory
    {
        // TODO: его нужно передать в виджет при создании, для размещения виджета на нужном лейауте
        private IWidgetCanvas _widgetCanvas;
        private IServiceResource _serviceResource;

        public static IWidgetFactory Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            return new WidgetFactory(widgetCanvas, serviceResource);
        }

        private WidgetFactory(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }
        
        bool IWidgetFactory.TryCreateWidget(JObject jsonData, out IWidget widget)
        {
            widget = null;
            
            if (!jsonData.TryGetValue("layout", out int layout))
            {
                return false;
            }

            var widgetType = (WidgetPlaceTypes) layout;
            var placeViewData = new WidgetPlaceViewData();
            if (placeViewData.TryParse(jsonData))
            {
                switch (widgetType)
                {
                    case WidgetPlaceTypes.None:
                        return false;
                    case WidgetPlaceTypes.Button:
                    case WidgetPlaceTypes.Title:
                    case WidgetPlaceTypes.Picture:
                        widget = new WidgetArea(_widgetCanvas, _serviceResource, placeViewData);
                        return true;
                    case WidgetPlaceTypes.Stacked:
                        widget = new WidgetStack(_widgetCanvas, _serviceResource, placeViewData);
                        return true;
                    case WidgetPlaceTypes.LayedOut:
                        widget = new WidgetDeck(_widgetCanvas, _serviceResource, placeViewData);
                        return true;
                }
            }

            return false;
        }

        void IWidgetFactory.Cleanup()
        {
            Cleanup();
        }

        void IWidgetFactory.Destroy()
        {
            // TODO: тут удаляем все что заинитили на создании
            Destroy();
            _widgetCanvas = null;
            _serviceResource = null;
        }

        private void Cleanup()
        {
            // TODO: тут нужно будет почистить пулы виджетов
            _widgetCanvas.GetWidgetPool().ReturnAllToPool();
        }

        private void Destroy()
        {
            _widgetCanvas.GetWidgetPool().Dispose();
        }
    }
}