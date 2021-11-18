using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Text
{
    public class WidgetCreatorText :  WidgetCreatorBase<WidgetTextViewData>
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;

        public WidgetCreatorText(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        protected override Widget Create()
        {
            return new WidgetText(_widgetCanvas, _serviceResource, _viewData);;
        }
    }
}