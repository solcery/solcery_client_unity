using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Card
{
    public class WidgetCreatorCard : WidgetCreatorBase<WidgetCardViewData>
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        
        public WidgetCreatorCard(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        protected override Widget Create()
        {
            return new WidgetCard(_widgetCanvas, _serviceResource, _viewData);;
        }
    }
}