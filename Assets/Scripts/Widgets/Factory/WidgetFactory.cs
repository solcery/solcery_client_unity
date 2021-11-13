using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.Widgets.Area;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Deck;

namespace Solcery.Widgets.Factory
{
    public sealed class WidgetFactory : IWidgetFactory
    {
        // TODO: его нужно передать в виджет при создании, для размещения виджета на нужном лейауте
        private IWidgetCanvas _widgetCanvas;

        public static IWidgetFactory Create(IWidgetCanvas widgetCanvas)
        {
            return new WidgetFactory(widgetCanvas);
        }

        private WidgetFactory(IWidgetCanvas widgetCanvas)
        {
            _widgetCanvas = widgetCanvas;
        }
        
        bool IWidgetFactory.TryCreateWidget(JObject jsonData, out Widget widget)
        {
            widget = null;
            
            if (!jsonData.TryGetValue("layout", out int layout))
            {
                return false;
            }

            var widgetType = (WidgetTypes) layout;
            var placeViewData = new WidgetPlaceViewData();
            if (placeViewData.TryParse(jsonData))
            {
                switch (widgetType)
                {
                    case WidgetTypes.None:
                        return false;
                    case WidgetTypes.Button:
                    case WidgetTypes.Title:
                    case WidgetTypes.Picture:
                        widget = new WidgetArea(_widgetCanvas, placeViewData);
                        return true;
                    case WidgetTypes.LayedOut:
                        widget = new WidgetDeck(_widgetCanvas, placeViewData);
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
            Cleanup();
            _widgetCanvas = null;
        }

        private void Cleanup()
        {
            // TODO: тут нужно будет почистить пулы виджетов
        }
    }
}