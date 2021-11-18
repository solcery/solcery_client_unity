using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Card
{
    public class WidgetCreatorCard : IWidgetCreator
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        private readonly WidgetCardViewData _viewData;
        
        public static WidgetCreatorCard Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            return new WidgetCreatorCard(widgetCanvas, serviceResource);
        }

        private WidgetCreatorCard(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _viewData = new WidgetCardViewData();
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        public bool TryCreate(JObject jsonData, out Widget widget)
        {
            if (_viewData.TryParse(jsonData))
            {
                widget = new WidgetCard(_widgetCanvas, _serviceResource, _viewData);
                return true;
            }

            widget = null;
            return false;
        }        
    }
}